using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderSetDefault(bool IsDefault) : DomainEvent, INotification;
