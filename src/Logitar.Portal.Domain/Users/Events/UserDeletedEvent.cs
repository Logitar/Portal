using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserDeletedEvent : DomainEvent, INotification
{
  public UserDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
    IsDeleted = true;
  }
}
