using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageDeletedEvent : DomainEvent, INotification
{
  public MessageDeletedEvent()
  {
    IsDeleted = true;
  }
}
