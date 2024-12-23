using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationInitialized(
  Locale? DefaultLocale,
  JwtSecret Secret,
  ReadOnlyUniqueNameSettings UniqueNameSettings,
  ReadOnlyPasswordSettings PasswordSettings,
  bool RequireUniqueEmail,
  ReadOnlyLoggingSettings LoggingSettings) : DomainEvent, INotification;
