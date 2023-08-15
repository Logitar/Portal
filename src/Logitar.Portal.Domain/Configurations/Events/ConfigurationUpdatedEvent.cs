using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationUpdatedEvent : DomainEvent, INotification
{
  public CultureInfo? DefaultLocale { get; set; }
  public string? Secret { get; set; }

  public ReadOnlyUniqueNameSettings? UniqueNameSettings { get; set; }
  public ReadOnlyPasswordSettings? PasswordSettings { get; set; }

  public ReadOnlyLoggingSettings? LoggingSettings { get; set; }
}
