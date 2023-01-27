using MediatR;

namespace Logitar.Portal.Core.Sessions.Events
{
  public class SessionSignedOutEvent : DomainEvent, INotification
  {
  }
}
