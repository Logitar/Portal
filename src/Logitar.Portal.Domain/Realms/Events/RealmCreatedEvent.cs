using Logitar.EventSourcing;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmCreatedEvent : DomainEvent, INotification
{
  public RealmCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string UniqueSlug { get; init; } = string.Empty;

  public JwtSecret Secret { get; init; } = JwtSecret.Generate();

  public bool RequireUniqueEmail { get; init; }
  public bool RequireConfirmedAccount { get; init; }

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; init; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; init; } = new();
}
