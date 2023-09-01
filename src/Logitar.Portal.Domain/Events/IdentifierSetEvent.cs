using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Events;

public abstract record IdentifierSetEvent : DomainEvent
{
  protected IdentifierSetEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string Key { get; init; } = string.Empty;
  public string Value { get; init; } = string.Empty;
}
