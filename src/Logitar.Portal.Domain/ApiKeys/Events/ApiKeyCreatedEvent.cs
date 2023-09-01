using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using MediatR;

namespace Logitar.Portal.Domain.ApiKeys.Events;

public record ApiKeyCreatedEvent : DomainEvent, INotification
{
  public ApiKeyCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public Password? Secret { get; init; }

  public string? TenantId { get; init; }

  public string Title { get; init; } = string.Empty;
}
