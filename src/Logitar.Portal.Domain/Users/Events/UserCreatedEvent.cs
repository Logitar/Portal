using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Users.Events;

public record UserCreatedEvent : DomainEvent, INotification
{
  public UserCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string? TenantId { get; init; }

  public string UniqueName { get; init; } = string.Empty;
}
