using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationUpdated : DomainEvent, INotification
{
  public Change<Locale>? DefaultLocale { get; set; }
  public JwtSecret? Secret { get; set; }

  public ReadOnlyUniqueNameSettings? UniqueNameSettings { get; set; }
  public ReadOnlyPasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }

  public ReadOnlyLoggingSettings? LoggingSettings { get; set; }

  [JsonIgnore]
  public bool HasChanges => DefaultLocale != null || Secret != null
    || UniqueNameSettings != null || PasswordSettings != null || RequireUniqueEmail.HasValue
    || LoggingSettings != null;
}
