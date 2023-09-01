using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.ApiKeys.Events;

public record ApiKeyDeletedEvent : DomainEvent, INotification
{
  public ApiKeyDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
