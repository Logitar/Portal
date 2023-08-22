using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationInitializedEvent : DomainEvent, INotification
{
  public Locale DefaultLocale { get; init; } = new("en");
  public string Secret { get; init; } = string.Empty;

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; init; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; init; } = new();

  public ReadOnlyLoggingSettings LoggingSettings { get; init; } = new();
}
