using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderSetDefaultEvent : DomainEvent, INotification
{
  public bool IsDefault { get; }

  public SenderSetDefaultEvent(ActorId actorId, bool isDefault)
  {
    ActorId = actorId;
    IsDefault = isDefault;
  }
}
