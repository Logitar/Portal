using MediatR;

namespace Logitar.Portal.Core2.Users.Events
{
  public class UserCreatedEvent : DomainEvent, INotification
  {
    public string Username { get; init; } = null!;
    public string? PasswordHash { get; init; }

    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }

    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }

    public string? Locale { get; init; }
    public string? Picture { get; init; }
  }
}
