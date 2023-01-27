using MediatR;

namespace Logitar.Portal.Core.Sessions.Events
{
  public class SessionDeletedEvent : DomainEvent, INotification
  {
  }
}
