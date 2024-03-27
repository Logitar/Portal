using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SmsSenderCreatedEvent : DomainEvent, INotification
{
  public TenantId? TenantId { get; }

  public PhoneUnit Phone { get; }

  public SenderProvider Provider { get; }

  public SmsSenderCreatedEvent(ActorId actorId, PhoneUnit phone, SenderProvider provider, TenantId? tenantId)
  {
    ActorId = actorId;
    Phone = phone;
    Provider = provider;
    TenantId = tenantId;
  }
}
