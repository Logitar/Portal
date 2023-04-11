using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Messages.Events;
using Logitar.Portal.v2.Core.Messages.Summaries;
using Logitar.Portal.v2.Core.Messages.Validators;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Senders;
using Logitar.Portal.v2.Core.Templates;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Messages;

public class MessageAggregate : AggregateRoot
{
  public MessageAggregate(AggregateId id) : base(id)
  {
  }

  public MessageAggregate(AggregateId actorId, RealmAggregate realm, SenderAggregate sender,
    TemplateAggregate template, string subject, string body, IEnumerable<Recipient> recipients,
    bool ignoreUserLocale = false, CultureInfo? locale = null,
    Dictionary<string, string>? variables = null, bool isDemo = false)
  {
    MessageCreated e = new()
    {
      ActorId = actorId,
      IsDemo = isDemo,
      Subject = subject,
      Body = body,
      Recipients = recipients,
      Realm = RealmSummary.From(realm),
      Sender = SenderSummary.From(sender),
      Template = TemplateSummary.From(template),
      IgnoreUserLocale = ignoreUserLocale,
      Locale = locale,
      Variables = variables ?? new()
    };
    new MessageCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  public bool IsDemo { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string Body { get; private set; } = string.Empty;

  public IEnumerable<Recipient> Recipients { get; private set; } = Enumerable.Empty<Recipient>();

  public RealmSummary Realm { get; private set; } = new();
  public SenderSummary Sender { get; private set; } = new();
  public TemplateSummary Template { get; private set; } = new();

  public bool IgnoreUserLocale { get; private set; }
  public CultureInfo? Locale { get; private set; }

  public Dictionary<string, string> Variables { get; private set; } = new();

  public bool HasErrors => false; // TODO(fpion): implement errors
  public bool Succeeded => false; // TODO(fpion): implement success

  protected virtual void Apply(MessageCreated e)
  {
    IsDemo = e.IsDemo;

    Subject = e.Subject;
    Body = e.Body;

    Recipients = e.Recipients;

    Realm = e.Realm;
    Sender = e.Sender;
    Template = e.Template;

    IgnoreUserLocale = e.IgnoreUserLocale;
    Locale = e.Locale;

    Variables = e.Variables;
  }

  public override string ToString() => $"{Subject} | {base.ToString()}";
}
