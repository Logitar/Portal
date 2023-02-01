using MediatR;

namespace Logitar.Portal.Domain.Users.Events
{
  public record UserCreatedEvent : DomainEvent, INotification
  {
    public AggregateId? RealmId { get; init; }

    public string Username { get; init; } = string.Empty;
    public string? PasswordHash { get; init; }

    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }

    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? FullName { get; init; }

    public string? LocaleName { get; init; }
    public string? Picture { get; init; }
  }
}
