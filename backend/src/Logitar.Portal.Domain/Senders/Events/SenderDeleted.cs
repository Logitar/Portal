using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderDeleted : DomainEvent, INotification
{
  public SenderDeleted()
  {
    IsDeleted = true;
  }
}
