using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Messages;

public class MessageAggregate : AggregateRoot
{
  public new MessageId Id => new(base.Id);

  public TenantId? TenantId { get; private set; }

  private SubjectUnit? _subject = null;
  public SubjectUnit Subject => _subject ?? throw new InvalidOperationException($"The {nameof(Subject)} has not been initialized yet.");
  private ContentUnit? _body = null;
  public ContentUnit Body => _body ?? throw new InvalidOperationException($"The {nameof(Body)} has not been initialized yet.");

  private readonly List<RecipientUnit> _recipients = [];
  public IReadOnlyCollection<RecipientUnit> Recipients => _recipients.AsReadOnly();
  private SenderSummary? _sender = null;
  public SenderSummary Sender => _sender ?? throw new InvalidOperationException($"The {nameof(Sender)} has not been initialized yet.");
  private TemplateSummary? _template = null;
  public TemplateSummary Template => _template ?? throw new InvalidOperationException($"The {nameof(Template)} has not been initialized yet.");

  public bool IgnoreUserLocale { get; private set; }
  public LocaleUnit? Locale { get; private set; }

  private readonly Dictionary<string, string> _variables = [];
  public IReadOnlyDictionary<string, string> Variables => _variables.AsReadOnly();

  public bool IsDemo { get; private set; }

  public MessageStatus Status { get; private set; }
  private readonly Dictionary<string, string> _result = [];
  public IReadOnlyDictionary<string, string> Result => _result.AsReadOnly();

  public MessageAggregate(AggregateId id) : base(id)
  {
  }

  public MessageAggregate(SubjectUnit subject, ContentUnit body, IReadOnlyCollection<RecipientUnit> recipients, SenderAggregate sender,
    TemplateAggregate template, bool ignoreUserLocale = false, LocaleUnit? locale = null, IReadOnlyDictionary<string, string>? variables = null,
    bool isDemo = false, TenantId? tenantId = null, ActorId actorId = default, MessageId? id = null) : base((id ?? MessageId.NewId()).AggregateId)
  {
    List<UserId> notInRealm = new(capacity: recipients.Count);
    int to = 0;
    foreach (RecipientUnit recipient in recipients)
    {
      if (recipient.Type == RecipientType.To)
      {
        to++;
      }

      if (recipient.User != null && recipient.User.TenantId != tenantId)
      {
        notInRealm.Add(recipient.User.Id);
      }
    }
    if (notInRealm.Count > 0)
    {
      throw new UsersNotInTenantException(notInRealm, tenantId);
    }
    else if (to == 0)
    {
      throw new ToRecipientMissingException(this, nameof(recipients));
    }

    if (sender.TenantId != tenantId)
    {
      throw new SenderNotInTenantException(sender, tenantId);
    }
    if (template.TenantId != tenantId)
    {
      throw new TemplateNotInTenantException(template, tenantId);
    }

    Raise(new MessageCreatedEvent(actorId, tenantId, subject, body, recipients, new SenderSummary(sender),
      new TemplateSummary(template), ignoreUserLocale, locale, variables ?? new Dictionary<string, string>(), isDemo));
  }
  protected virtual void Apply(MessageCreatedEvent @event)
  {
    TenantId = @event.TenantId;

    _subject = @event.Subject;
    _body = @event.Body;

    _recipients.AddRange(@event.Recipients);
    _sender = @event.Sender;
    _template = @event.Template;

    IgnoreUserLocale = @event.IgnoreUserLocale;
    Locale = @event.Locale;

    _variables.AddRange(@event.Variables);

    IsDemo = @event.IsDemo;

    Status = MessageStatus.Unsent;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new MessageDeletedEvent(actorId));
    }
  }

  public override string ToString() => $"{Subject.Value} | {base.ToString()}";
}
