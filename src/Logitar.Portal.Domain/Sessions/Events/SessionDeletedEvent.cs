using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Sessions.Events;

public record SessionDeletedEvent : DomainEvent, INotification
{
  public SessionDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
