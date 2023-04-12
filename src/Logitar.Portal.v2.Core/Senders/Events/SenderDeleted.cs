using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Events;

public record SenderDeleted : DomainEvent, INotification
{
  public SenderDeleted() => DeleteAction = DeleteAction.Delete;
}
