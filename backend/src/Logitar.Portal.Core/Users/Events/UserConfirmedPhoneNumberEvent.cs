using MediatR;

namespace Logitar.Portal.Core.Users.Events
{
  public class UserConfirmedPhoneNumberEvent : DomainEvent, INotification
  {
  }
}
