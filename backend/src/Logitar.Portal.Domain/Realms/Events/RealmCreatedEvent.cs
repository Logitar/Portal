using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events
{
  public record RealmCreatedEvent : DomainEvent, INotification
  {
    public string Alias { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public string? Description { get; init; }

    public string? DefaultLocaleName { get; init; }
    public string JwtSecret { get; init; } = string.Empty;
    public string? Url { get; init; }

    public bool RequireConfirmedAccount { get; init; }
    public bool RequireUniqueEmail { get; init; }

    public UsernameSettings UsernameSettings { get; init; } = new();
    public PasswordSettings PasswordSettings { get; init; } = new();

    public string? GoogleClientId { get; init; }
  }
}
