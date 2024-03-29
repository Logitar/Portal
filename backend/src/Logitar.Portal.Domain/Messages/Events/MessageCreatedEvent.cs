using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageCreatedEvent(TenantId? TenantId, SubjectUnit Subject, ContentUnit Body, IReadOnlyCollection<RecipientUnit> Recipients,
  SenderSummary Sender, TemplateSummary Template, bool IgnoreUserLocale, LocaleUnit? Locale, IReadOnlyDictionary<string, string> Variables, bool IsDemo)
  : DomainEvent, INotification;
