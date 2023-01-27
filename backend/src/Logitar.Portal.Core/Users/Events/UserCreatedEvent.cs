using MediatR;

namespace Logitar.Portal.Core.Users.Events
{
  public class UserCreatedEvent : DomainEvent, INotification
  {
    public string? RealmId { get; init; }

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
