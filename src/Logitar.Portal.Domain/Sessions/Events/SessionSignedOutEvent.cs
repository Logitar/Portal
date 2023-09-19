using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Sessions.Events;

public record SessionSignedOutEvent : DomainEvent, INotification
{
  public SessionSignedOutEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
