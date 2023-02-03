using Logitar.Portal.Domain.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Domain.Realms.Events
{
  public record RealmUpdatedEvent : DomainEvent, INotification
  {
    public string? DisplayName { get; init; }
    public string? Description { get; init; }

    public CultureInfo? DefaultLocale { get; init; }
    public string JwtSecret { get; init; } = string.Empty;
    public string? Url { get; init; }

    public bool RequireConfirmedAccount { get; init; }
    public bool RequireUniqueEmail { get; init; }

    public UsernameSettings UsernameSettings { get; init; } = new();
    public PasswordSettings PasswordSettings { get; init; } = new();

    public string? GoogleClientId { get; init; }
  }
}
