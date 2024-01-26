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

  public RealmCreatedEvent(ActorId actorId, UniqueSlugUnit uniqueSlug)
  {
    ActorId = actorId;

    UniqueSlug = uniqueSlug;

    Secret = JwtSecretUnit.Generate();

    UniqueNameSettings = new();
    PasswordSettings = new();
    RequireUniqueEmail = true;
  }

  [JsonConstructor]
  internal RealmCreatedEvent(ActorId actorId, ReadOnlyPasswordSettings passwordSettings, bool requireUniqueEmail, JwtSecretUnit secret,
    ReadOnlyUniqueNameSettings uniqueNameSettings, UniqueSlugUnit uniqueSlug)
  {
    ActorId = actorId;

    UniqueSlug = uniqueSlug;

    Secret = secret;

    UniqueNameSettings = uniqueNameSettings;
    PasswordSettings = passwordSettings;
    RequireUniqueEmail = requireUniqueEmail;
  }
}
