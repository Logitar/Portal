using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Users.Events;

public record UserCreatedEvent : DomainEvent
{
  public UserCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string? TenantId { get; init; }

  public string UniqueName { get; init; } = string.Empty;
}
