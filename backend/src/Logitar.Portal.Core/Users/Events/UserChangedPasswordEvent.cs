using MediatR;

namespace Logitar.Portal.Core.Users.Events
{
  public class UserChangedPasswordEvent : DomainEvent, INotification
  {
    public string PasswordHash { get; init; } = null!;
  }
}
