using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Senders.Events;

public record SenderDeleted : DomainEvent, INotification
{
  public SenderDeleted() => DeleteAction = DeleteAction.Delete;
}
