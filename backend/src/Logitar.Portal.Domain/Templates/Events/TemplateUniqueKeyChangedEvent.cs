using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateUniqueKeyChangedEvent : DomainEvent, INotification
{
  public UniqueKeyUnit UniqueKey { get; }

  public TemplateUniqueKeyChangedEvent(ActorId actorId, UniqueKeyUnit uniqueKey)
  {
    ActorId = actorId;
    UniqueKey = uniqueKey;
  }
}
