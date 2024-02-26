using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmUniqueSlugChangedEvent : DomainEvent, INotification
{
  public UniqueSlugUnit UniqueSlug { get; }

  public RealmUniqueSlugChangedEvent(ActorId actorId, UniqueSlugUnit uniqueSlug)
  {
    ActorId = actorId;
    UniqueSlug = uniqueSlug;
  }
}
