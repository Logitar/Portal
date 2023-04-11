using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Events;

public record SenderSetDefault : DomainEvent, INotification
{
  public bool IsDefault { get; init; }
}
