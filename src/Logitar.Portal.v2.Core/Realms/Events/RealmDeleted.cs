using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Events;

public record RealmDeleted : DomainEvent, INotification
{
  public RealmDeleted() => DeleteAction = DeleteAction.Delete;
}
