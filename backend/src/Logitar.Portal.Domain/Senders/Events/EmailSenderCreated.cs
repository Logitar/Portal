using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record EmailSenderCreated : SenderCreated, INotification
{
  public EmailSenderCreated(Email email, SenderProvider provider) : base(email, provider)
  {
  }
}
