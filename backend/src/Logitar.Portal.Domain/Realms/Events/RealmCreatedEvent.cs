using Logitar.EventSourcing;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmCreatedEvent : DomainEvent, INotification
{
  public UniqueSlugUnit UniqueSlug { get; }

  public JwtSecretUnit Secret { get; }

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; }
  public ReadOnlyPasswordSettings PasswordSettings { get; }
  public bool RequireUniqueEmail { get; }

  public RealmCreatedEvent(ActorId actorId, UniqueSlugUnit uniqueSlug, JwtSecretUnit secret, ReadOnlyUniqueNameSettings uniqueNameSettings,
    ReadOnlyPasswordSettings passwordSettings, bool requireUniqueEmail)
  {
    ActorId = actorId;
    UniqueSlug = uniqueSlug;
    Secret = secret;
    UniqueNameSettings = uniqueNameSettings;
    PasswordSettings = passwordSettings;
    RequireUniqueEmail = requireUniqueEmail;
  }
}
