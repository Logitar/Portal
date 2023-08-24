using Logitar.EventSourcing;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationUpdatedEvent : DomainEvent
{
  public Locale? DefaultLocale { get; set; }
  public string? Secret { get; set; }

  public ReadOnlyUniqueNameSettings? UniqueNameSettings { get; set; }
  public ReadOnlyPasswordSettings? PasswordSettings { get; set; }

  public ReadOnlyLoggingSettings? LoggingSettings { get; set; }
}
