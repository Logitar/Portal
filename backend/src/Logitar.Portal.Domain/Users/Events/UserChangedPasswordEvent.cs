using MediatR;

namespace Logitar.Portal.Domain.Users.Events
{
  public record UserChangedPasswordEvent : DomainEvent, INotification
  {
    public string PasswordHash { get; init; } = string.Empty;
  }
}
