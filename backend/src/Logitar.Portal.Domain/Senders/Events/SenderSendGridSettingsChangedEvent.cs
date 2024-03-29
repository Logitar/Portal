using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders.SendGrid;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderSendGridSettingsChangedEvent(ReadOnlySendGridSettings Settings) : DomainEvent, INotification;
