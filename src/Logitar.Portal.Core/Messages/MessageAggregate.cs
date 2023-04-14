using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core.Messages.Events;
using Logitar.Portal.Core.Messages.Summaries;
using Logitar.Portal.Core.Messages.Validators;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Templates;
using System.Globalization;

namespace Logitar.Portal.Core.Messages;

public class MessageAggregate : AggregateRoot
{
  private readonly List<Error> _errors = new();
  private SendMessageResult? _result = null;

  public MessageAggregate(AggregateId id) : base(id)
  {
  }

  public MessageAggregate(AggregateId actorId, RealmAggregate? realm, SenderAggregate sender,
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

  public RealmSummary? Realm { get; private set; }
  public SenderSummary Sender { get; private set; } = new();
  public TemplateSummary Template { get; private set; } = new();

  public bool IgnoreUserLocale { get; private set; }
  public CultureInfo? Locale { get; private set; }

  public Dictionary<string, string> Variables { get; private set; } = new();

  public IReadOnlyCollection<Error> Errors => _errors.AsReadOnly();
  public bool HasErrors => Errors.Any();

  public IReadOnlyDictionary<string, string>? Result => _result?.AsReadOnly();
  public bool Succeeded => !HasErrors && Result != null;

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

  public void Fail(Error error)
  {
    ApplyChange(new MessageFailed
    {
      ActorId = new AggregateId(Guid.Empty),
      Errors = new[] { error }
    });
  }
  protected virtual void Apply(MessageFailed e)
  {
    _errors.Clear();
    _errors.AddRange(e.Errors);
  }

  public void Succeed(SendMessageResult result)
  {
    ApplyChange(new MessageSucceeded
    {
      ActorId = new AggregateId(Guid.Empty),
      Result = result
    });
  }
  protected virtual void Apply(MessageSucceeded e) => _result = e.Result;

  public override string ToString() => $"{Subject} | {base.ToString()}";
}
