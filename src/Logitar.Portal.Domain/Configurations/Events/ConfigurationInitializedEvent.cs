using Logitar.EventSourcing;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationInitializedEvent : DomainEvent
{
  public ConfigurationInitializedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public Locale DefaultLocale { get; init; } = Locale.Default;
  public string Secret { get; init; } = string.Empty;

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; init; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; init; } = new();

  public ReadOnlyLoggingSettings LoggingSettings { get; init; } = new();
}
