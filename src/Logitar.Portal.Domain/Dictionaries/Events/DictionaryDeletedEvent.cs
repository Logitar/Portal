using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryDeletedEvent : DomainEvent, INotification
{
  public DictionaryDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }
}
