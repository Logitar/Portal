using MediatR;

namespace Logitar.Portal.Core.Users.Events
{
  public class UserConfirmedEmailEvent : DomainEvent, INotification
  {
  }
}
