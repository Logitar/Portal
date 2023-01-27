using MediatR;

namespace Logitar.Portal.Core.Users.Events
{
  public class UserUpdatedEvent : DomainEvent, INotification
  {
    public string? PasswordHash { get; init; }

    public bool HasEmailChanged { get; init; }
    public string? Email { get; init; }
    public bool HasPhoneNumberChanged { get; init; }
    public string? PhoneNumber { get; init; }

    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }

    public string? Locale { get; init; }
    public string? Picture { get; init; }
  }
}
