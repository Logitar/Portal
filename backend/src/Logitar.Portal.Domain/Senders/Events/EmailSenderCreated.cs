using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record EmailSenderCreated : SenderCreated, INotification
{
  public EmailSenderCreated(TenantId? tenantId, EmailUnit email, SenderProvider provider) : base(tenantId, email, provider)
  {
  }
}
