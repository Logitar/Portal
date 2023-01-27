using MediatR;

namespace Logitar.Portal.Core.Realms.Events
{
  public class RealmDeletedEvent : DomainEvent, INotification
  {
  }
}
