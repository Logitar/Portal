using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;

namespace Logitar.Portal.Domain.Sessions.Events;

public record SessionCreatedEvent : DomainEvent
{
  public SessionCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public AggregateId UserId { get; init; }

  public Password? Secret { get; init; }
}
