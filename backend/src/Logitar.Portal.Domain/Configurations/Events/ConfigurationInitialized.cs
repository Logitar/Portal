using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationInitialized(
  LocaleUnit? DefaultLocale,
  JwtSecret Secret,
  ReadOnlyUniqueNameSettings UniqueNameSettings,
  ReadOnlyPasswordSettings PasswordSettings,
  bool RequireUniqueEmail,
  ReadOnlyLoggingSettings LoggingSettings) : DomainEvent, INotification;
