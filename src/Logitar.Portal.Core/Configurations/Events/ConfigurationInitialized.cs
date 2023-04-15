using Logitar.EventSourcing;
using Logitar.Portal.Core.Realms;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Configurations.Events;

public record ConfigurationInitialized : DomainEvent, INotification
{
  public CultureInfo DefaultLocale { get; init; } = null!;
  public string Secret { get; init; } = string.Empty;

  public ReadOnlyUsernameSettings UsernameSettings { get; init; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; init; } = new();

  public ReadOnlyLoggingSettings LoggingSettings { get; init; } = new();
}
