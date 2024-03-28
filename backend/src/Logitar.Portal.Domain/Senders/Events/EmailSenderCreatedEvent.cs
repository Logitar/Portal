using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record EmailSenderCreatedEvent : SenderCreatedEvent, INotification
{
  public EmailSenderCreatedEvent(ActorId actorId, EmailUnit email, SenderProvider provider, TenantId? tenantId) : base(actorId, email, provider, tenantId)
  {
  }
}
