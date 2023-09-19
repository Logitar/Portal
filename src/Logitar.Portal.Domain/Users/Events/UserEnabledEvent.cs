using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserEnabledEvent : DomainEvent, INotification
{
  public UserEnabledEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
