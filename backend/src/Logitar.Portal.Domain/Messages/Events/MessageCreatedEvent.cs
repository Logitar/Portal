using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageCreatedEvent : DomainEvent, INotification
{
  public TenantId? TenantId { get; }

  public SubjectUnit Subject { get; }
  public ContentUnit Body { get; }

  public IReadOnlyCollection<RecipientUnit> Recipients { get; }
  public SenderSummary Sender { get; }
  public TemplateSummary Template { get; }

  public bool IgnoreUserLocale { get; }
  public LocaleUnit? Locale { get; }

  public IReadOnlyDictionary<string, string> Variables { get; }

  public bool IsDemo { get; }

  public MessageCreatedEvent(ActorId actorId, TenantId? tenantId, SubjectUnit subject, ContentUnit body, IReadOnlyCollection<RecipientUnit> recipients,
    SenderSummary sender, TemplateSummary template, bool ignoreUserLocale, LocaleUnit? locale, IReadOnlyDictionary<string, string> variables, bool isDemo)
  {
    ActorId = actorId;
    TenantId = tenantId;
    Subject = subject;
    Body = body;
    Recipients = recipients;
    Sender = sender;
    Template = template;
    IgnoreUserLocale = ignoreUserLocale;
    Locale = locale;
    Variables = variables;
    IsDemo = isDemo;
  }
}
