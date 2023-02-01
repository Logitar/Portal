using MediatR;

namespace Logitar.Portal.Domain.Users.Events
{
  public record UserUpdatedEvent : DomainEvent, INotification
  {
    public string? PasswordHash { get; init; }

    public bool HasEmailChanged { get; init; }
    public string? Email { get; init; }
    public bool HasPhoneNumberChanged { get; init; }
    public string? PhoneNumber { get; init; }

    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? FullName { get; init; }

    public string? LocaleName { get; init; }
    public string? Picture { get; init; }
  }
}
