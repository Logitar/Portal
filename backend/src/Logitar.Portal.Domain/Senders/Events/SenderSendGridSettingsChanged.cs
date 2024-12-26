using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders.SendGrid;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderSendGridSettingsChanged(ReadOnlySendGridSettings Settings) : DomainEvent, INotification;
