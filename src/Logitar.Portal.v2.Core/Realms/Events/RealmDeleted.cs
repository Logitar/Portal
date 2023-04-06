using Logitar.EventSourcing;

namespace Logitar.Portal.v2.Core.Realms.Events;

internal record RealmDeleted : DomainEvent
{
  public RealmDeleted() => DeleteAction = DeleteAction.Delete;
}
