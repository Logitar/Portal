using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Senders.Events;

public record SenderSetDefault : DomainEvent, INotification
{
  public bool IsDefault { get; init; }
}
