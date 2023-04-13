using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record UserDeleted : DomainEvent, INotification
{
  public UserDeleted() => DeleteAction = DeleteAction.Delete;
}
