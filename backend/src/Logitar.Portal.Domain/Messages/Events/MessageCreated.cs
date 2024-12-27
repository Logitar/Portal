using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageCreated(
  Subject Subject,
  Content Body,
  IReadOnlyCollection<Recipient> Recipients,
  SenderSummary Sender,
  TemplateSummary Template,
  bool IgnoreUserLocale,
  Locale? Locale,
  IReadOnlyDictionary<string, string> Variables,
  bool IsDemo) : DomainEvent, INotification;
