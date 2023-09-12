using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderCreatedEvent : DomainEvent, INotification
{
  public SenderCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string? TenantId { get; init; }

  public bool IsDefault { get; init; }

  public ProviderType Provider { get; init; }

  public string EmailAddress { get; init; } = string.Empty;
}
