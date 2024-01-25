using Logitar.EventSourcing;
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
    PasswordSettings = new();
    RequireUniqueEmail = true;
    Secret = JwtSecretUnit.Generate();
    UniqueNameSettings = new();
    UniqueSlug = uniqueSlug;
  }

  [JsonConstructor]
  internal RealmCreatedEvent(ActorId actorId, ReadOnlyPasswordSettings passwordSettings, bool requireUniqueEmail, JwtSecretUnit secret,
    ReadOnlyUniqueNameSettings uniqueNameSettings, UniqueSlugUnit uniqueSlug)
  {
    ActorId = actorId;
    PasswordSettings = passwordSettings;
    RequireUniqueEmail = requireUniqueEmail;
    Secret = secret;
    UniqueNameSettings = uniqueNameSettings;
    UniqueSlug = uniqueSlug;
  }
}
