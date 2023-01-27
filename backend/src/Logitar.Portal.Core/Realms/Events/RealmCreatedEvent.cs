using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Realms.Events
{
  public class RealmCreatedEvent : DomainEvent, INotification
  {
    public string Alias { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public string? Description { get; init; }

    public string? DefaultLocale { get; init; }
    public string? Url { get; init; }

    public bool RequireConfirmedAccount { get; init; }
    public bool RequireUniqueEmail { get; init; }

    public UsernameSettings UsernameSettings { get; init; } = null!;
    public PasswordSettings PasswordSettings { get; init; } = null!;

    public string? GoogleClientId { get; init; }
  }
}
