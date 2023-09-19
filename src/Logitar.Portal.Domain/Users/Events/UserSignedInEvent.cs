using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserSignedInEvent : DomainEvent, INotification
{
  public UserSignedInEvent(ActorId actorId, DateTime occurredOn)
  {
    ActorId = actorId;
    OccurredOn = occurredOn;
  }
}
