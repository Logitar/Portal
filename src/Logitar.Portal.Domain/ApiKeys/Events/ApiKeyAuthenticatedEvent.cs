using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.ApiKeys.Events;

public record ApiKeyAuthenticatedEvent : DomainEvent, INotification
{
  public ApiKeyAuthenticatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
