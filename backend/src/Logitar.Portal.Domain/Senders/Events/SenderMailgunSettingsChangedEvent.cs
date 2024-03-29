using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders.Mailgun;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderMailgunSettingsChangedEvent(ReadOnlyMailgunSettings Settings) : DomainEvent, INotification;
