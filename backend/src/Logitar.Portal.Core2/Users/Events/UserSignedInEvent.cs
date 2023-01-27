using MediatR;

namespace Logitar.Portal.Core2.Users.Events
{
  public class UserSignedInEvent : DomainEvent, INotification
  {
  }
}
