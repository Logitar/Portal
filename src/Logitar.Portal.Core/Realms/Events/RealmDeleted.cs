using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Realms.Events;

public record RealmDeleted : DomainEvent, INotification
{
  public RealmDeleted() => DeleteAction = DeleteAction.Delete;
}
