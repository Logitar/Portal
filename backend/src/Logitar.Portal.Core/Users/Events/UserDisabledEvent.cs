using MediatR;

namespace Logitar.Portal.Core.Users.Events
{
  public class UserDisabledEvent : DomainEvent, INotification
  {
  }
}
