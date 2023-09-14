using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;

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
    // TODO(fpion): validate subject, body, recipients/realm, sender/realm, template/realm

    ApplyChange(new MessageCreatedEvent(actorId)
    {
      Subject = subject,
      Body = body,
      Recipients = recipients,
      RealmId = realm?.Id,
      SenderId = sender.Id,
      TemplateId = template.Id,
      IgnoreUserLocale = ignoreUserLocale,
      Locale = locale,
      Variables = variables ?? new(),
      IsDemo = isDemo
    });
  }
  protected virtual void Apply(MessageCreatedEvent created)
  {
    Subject = created.Subject;
    Body = created.Body;

    Recipients = created.Recipients;

    RealmId = created.RealmId;
    SenderId = created.SenderId;
    TemplateId = created.TemplateId;

    IgnoreUserLocale = created.IgnoreUserLocale;
    Locale = created.Locale;

    Variables = created.Variables;

    IsDemo = created.IsDemo;
  }

  public string Subject { get; private set; } = string.Empty;
  public string Body { get; private set; } = string.Empty;

  public Recipients Recipients { get; private set; } = new();

  public AggregateId? RealmId { get; private set; }
  public AggregateId SenderId { get; private set; }
  public AggregateId TemplateId { get; private set; }

  public bool IgnoreUserLocale { get; private set; }
  public Locale? Locale { get; private set; }

  public Variables Variables { get; private set; } = new();

  public bool IsDemo { get; private set; }

  // TODO(fpion): Error
  // TODO(fpion): Result
  public MessageStatus Status { get; private set; }

  public override string ToString() => $"{Subject} | {base.ToString()}";
}
