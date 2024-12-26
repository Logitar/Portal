using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageCreated(
  TenantId? TenantId,
  Subject Subject,
  Content Body,
  IReadOnlyCollection<Recipient> Recipients,
  SenderSummary Sender,
  TemplateSummary Template,
  bool IgnoreUserLocale,
  LocaleUnit? Locale,
  IReadOnlyDictionary<string, string> Variables,
  bool IsDemo) : DomainEvent, INotification;
