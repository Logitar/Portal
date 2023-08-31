using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Roles.Events;

public record RoleDeletedEvent : DomainEvent, INotification
{
  public RoleDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
