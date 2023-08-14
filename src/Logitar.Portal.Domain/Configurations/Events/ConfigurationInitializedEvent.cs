using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationInitializedEvent : DomainEvent, INotification
{
  public CultureInfo DefaultLocale { get; init; } = CultureInfo.InvariantCulture;
  public string Secret { get; init; } = string.Empty;

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; init; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; init; } = new();

  public ReadOnlyLoggingSettings LoggingSettings { get; init; } = new();
}
