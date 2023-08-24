using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Users.Events;

public record UserSignedInEvent : DomainEvent
{
  public UserSignedInEvent(ActorId actorId, DateTime occurredOn)
  {
    ActorId = actorId;
    OccurredOn = occurredOn;
  }
}
