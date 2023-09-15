using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Templates.Validators;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Messages;

public class MessageAggregate : AggregateRoot
{
  public MessageAggregate(AggregateId id) : base(id)
  {
  }

  public MessageAggregate(string subject, string body, Recipients recipients, SenderAggregate sender,
    TemplateAggregate template, RealmAggregate? realm = null, bool ignoreUserLocale = false,
    Locale? locale = null, Variables? variables = null, bool isDemo = false, ActorId actorId = default,
    AggregateId? id = null) : base(id)
  {
    subject = subject.Trim();
    new SubjectValidator(nameof(Subject)).ValidateAndThrow(subject);

    body = body.Trim();
    new ContentsValidator(nameof(Body)).ValidateAndThrow(body);

    IEnumerable<UserAggregate> usersNotInRealm = recipients.AsEnumerable()
      .Where(x => x.User != null && x.User.TenantId != realm?.Id.Value)
      .Select(x => x.User!);
    if (usersNotInRealm.Any())
    {
      throw new UsersNotInRealmException(usersNotInRealm, realm, nameof(Recipients));
    }

    if (sender.TenantId != realm?.Id.Value)
    {
      throw new SenderNotInRealmException(sender, realm, nameof(Sender));
    }

    if (template.TenantId != realm?.Id.Value)
    {
      throw new TemplateNotInRealmException(template, realm, nameof(Template));
    }

    ApplyChange(new MessageCreatedEvent(actorId)
    {
      Subject = subject,
      Body = body,
      Recipients = recipients.AsEnumerable(),
      Realm = realm == null ? null : RealmSummary.From(realm),
      Sender = SenderSummary.From(sender),
      Template = TemplateSummary.From(template),
      IgnoreUserLocale = ignoreUserLocale,
      Locale = locale,
      Variables = variables?.AsDictionary() ?? new(),
      IsDemo = isDemo
    });
  }
  protected virtual void Apply(MessageCreatedEvent created)
  {
    Subject = created.Subject;
    Body = created.Body;

    Recipients = new Recipients(created.Recipients);

    Realm = created.Realm;
    Sender = created.Sender;
    Template = created.Template;

    IgnoreUserLocale = created.IgnoreUserLocale;
    Locale = created.Locale;

    Variables = new Variables(created.Variables);

    IsDemo = created.IsDemo;

    Status = MessageStatus.Unsent;
  }

  public string Subject { get; private set; } = string.Empty;
  public string Body { get; private set; } = string.Empty;

  public Recipients Recipients { get; private set; } = new();

  public RealmSummary? Realm { get; private set; }
  public SenderSummary Sender { get; private set; } = new();
  public TemplateSummary Template { get; private set; } = new();

  public bool IgnoreUserLocale { get; private set; }
  public Locale? Locale { get; private set; }

  public Variables Variables { get; private set; } = new();

  public bool IsDemo { get; private set; }

  public SendMessageResult? Result { get; private set; }
  public MessageStatus Status { get; private set; }

  public void Fail(SendMessageResult result, ActorId actorId = default)
  {
    ApplyChange(new MessageFailedEvent(actorId)
    {
      Result = result.AsDictionary()
    });
  }
  protected virtual void Apply(MessageFailedEvent failed)
  {
    Result = new SendMessageResult(failed.Result);
    Status = MessageStatus.Failed;
  }

  public void Succeed(SendMessageResult result, ActorId actorId = default)
  {
    ApplyChange(new MessageSucceededEvent(actorId)
    {
      Result = result.AsDictionary()
    });
  }
  protected virtual void Apply(MessageSucceededEvent succeeded)
  {
    Result = new SendMessageResult(succeeded.Result);
    Status = MessageStatus.Succeeded;
  }

  public override string ToString() => $"{Subject} | {base.ToString()}";
}
