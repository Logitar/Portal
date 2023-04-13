using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Events;

public record SessionDeleted : DomainEvent, INotification
{
  public SessionDeleted() => DeleteAction = DeleteAction.Delete;
}
