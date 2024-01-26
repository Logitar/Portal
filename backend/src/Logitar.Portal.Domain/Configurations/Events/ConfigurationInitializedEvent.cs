using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationInitializedEvent : DomainEvent, INotification
{
  public LocaleUnit? DefaultLocale { get; }
  public JwtSecretUnit Secret { get; }

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; }
  public ReadOnlyPasswordSettings PasswordSettings { get; }
  public bool RequireUniqueEmail { get; }

  public ConfigurationInitializedEvent(ActorId actorId, LocaleUnit? defaultLocale)
  {
    ActorId = actorId;

    DefaultLocale = defaultLocale;
    Secret = JwtSecretUnit.Generate();

    UniqueNameSettings = new();
    PasswordSettings = new();
    RequireUniqueEmail = true;
  }

  [JsonConstructor]
  public ConfigurationInitializedEvent(ActorId actorId, LocaleUnit? defaultLocale, ReadOnlyPasswordSettings passwordSettings, bool requireUniqueEmail,
    JwtSecretUnit secret, ReadOnlyUniqueNameSettings uniqueNameSettings)
  {
    ActorId = actorId;

    DefaultLocale = defaultLocale;
    Secret = secret;

    UniqueNameSettings = uniqueNameSettings;
    PasswordSettings = passwordSettings;
    RequireUniqueEmail = requireUniqueEmail;
  }
}
