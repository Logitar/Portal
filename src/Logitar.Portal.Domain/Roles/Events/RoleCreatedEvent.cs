using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Roles.Events;

public record RoleCreatedEvent : DomainEvent, INotification
{
  public RoleCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string? TenantId { get; init; }

  public string UniqueName { get; init; } = string.Empty;
}
