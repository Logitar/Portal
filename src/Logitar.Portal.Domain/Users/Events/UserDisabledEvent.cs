using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserDisabledEvent : DomainEvent, INotification
{
  public UserDisabledEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
