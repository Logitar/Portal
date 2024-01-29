﻿using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events;

public record ConfigurationUpdatedEvent : DomainEvent, INotification
{
  public Modification<LocaleUnit>? DefaultLocale { get; set; }
  public JwtSecretUnit? Secret { get; set; }

  public ReadOnlyUniqueNameSettings? UniqueNameSettings { get; set; }
  public ReadOnlyPasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }

  public ReadOnlyLoggingSettings? LoggingSettings { get; set; }

  [JsonIgnore]
  public bool HasChanges => DefaultLocale != null || Secret != null
    || UniqueNameSettings != null || PasswordSettings != null || RequireUniqueEmail.HasValue
    || LoggingSettings != null;
}
