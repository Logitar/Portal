using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderSetDefaultEvent(bool IsDefault) : DomainEvent, INotification;
