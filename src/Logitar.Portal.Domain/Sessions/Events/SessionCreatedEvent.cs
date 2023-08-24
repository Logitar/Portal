using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using MediatR;

namespace Logitar.Portal.Domain.Sessions.Events;

public record SessionCreatedEvent : DomainEvent, INotification
{
  public SessionCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public AggregateId UserId { get; init; }

  public Password? Secret { get; init; }
}
