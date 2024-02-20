using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderCreatedEvent : DomainEvent, INotification
{
  public TenantId? TenantId { get; }

  public EmailUnit Email { get; }

  public SenderProvider Provider { get; }

  public SenderCreatedEvent(ActorId actorId, EmailUnit email, SenderProvider provider, TenantId? tenantId)
  {
    ActorId = actorId;
    Email = email;
    Provider = provider;
    TenantId = tenantId;
  }
}
