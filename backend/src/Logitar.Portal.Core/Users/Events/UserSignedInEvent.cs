using MediatR;

namespace Logitar.Portal.Core.Users.Events
{
  public class UserSignedInEvent : DomainEvent, INotification
  {
  }
}
