using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmCreatedEvent : DomainEvent, INotification
{
  public string UniqueSlug { get; init; } = string.Empty;

  public string Secret { get; init; } = string.Empty;

  public bool RequireUniqueEmail { get; init; }
  public bool RequireConfirmedAccount { get; init; }

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; init; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; init; } = new();
}
