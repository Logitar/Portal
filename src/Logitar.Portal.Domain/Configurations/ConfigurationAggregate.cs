using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Validators;
using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Configurations.Validators;
using Logitar.Portal.Domain.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  public static readonly AggregateId AggregateId = new("CONFIGURATION");

  private CultureInfo _defaultLocale = CultureInfo.InvariantCulture;
  private string _secret = string.Empty;

  private ReadOnlyUniqueNameSettings _uniqueNameSettings = new();
  private ReadOnlyPasswordSettings _passwordSettings = new();

  private ReadOnlyLoggingSettings _loggingSettings = new();

  public ConfigurationAggregate(AggregateId id) : base(id)
  {
  }

  public ConfigurationAggregate(CultureInfo defaultLocale, ActorId actorId) : base(AggregateId)
  {
    ConfigurationInitializedEvent initialized = new()
    {
      ActorId = actorId,
      DefaultLocale = defaultLocale,
      Secret = RandomStringGenerator.GetString()
    };
    new ConfigurationInitializedValidator().ValidateAndThrow(initialized);

    ApplyChange(initialized);
  }
  protected virtual void Apply(ConfigurationInitializedEvent initialized)
  {
    _defaultLocale = initialized.DefaultLocale;
    _secret = initialized.Secret;

    UniqueNameSettings = initialized.UniqueNameSettings;
    PasswordSettings = initialized.PasswordSettings;

    LoggingSettings = initialized.LoggingSettings;
  }

  public CultureInfo DefaultLocale
  {
    get => _defaultLocale;
    set
    {
      new LocaleValidator(nameof(DefaultLocale)).ValidateAndThrow(value);

      if (value != _defaultLocale)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.DefaultLocale = value;
        Apply(updated);
      }
    }
  }
  public string Secret
  {
    get => _secret;
    set
    {
      value = string.IsNullOrWhiteSpace(value) ? RandomStringGenerator.GetString() : value.Trim();
      new SecretValidator(nameof(Secret)).ValidateAndThrow(value);

      if (value != _secret)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.Secret = value;
        Apply(updated);
      }
    }
  }

  public ReadOnlyUniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings;
    set
    {
      new ReadOnlyUniqueNameSettingsValidator().ValidateAndThrow(value);

      if (value != _uniqueNameSettings)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.UniqueNameSettings = value;
        Apply(updated);
      }
    }
  }
  public ReadOnlyPasswordSettings PasswordSettings
  {
    get => _passwordSettings;
    set
    {
      new ReadOnlyPasswordSettingsValidator().ValidateAndThrow(value);

      if (value != _passwordSettings)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.PasswordSettings = value;
        Apply(updated);
      }
    }
  }

  public ReadOnlyLoggingSettings LoggingSettings
  {
    get => _loggingSettings;
    set
    {
      new ReadOnlyLoggingSettingsValidator().ValidateAndThrow(value);

      if (value != _loggingSettings)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.LoggingSettings = value;
        Apply(updated);
      }
    }
  }

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is ConfigurationUpdatedEvent)
      {
        change.ActorId = actorId;

        if (change.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }
  protected virtual void Apply(ConfigurationUpdatedEvent updated)
  {
    if (updated.DefaultLocale != null)
    {
      _defaultLocale = updated.DefaultLocale;
    }
    if (updated.Secret != null)
    {
      _secret = updated.Secret;
    }

    if (updated.UniqueNameSettings != null)
    {
      _uniqueNameSettings = updated.UniqueNameSettings;
    }
    if (updated.PasswordSettings != null)
    {
      _passwordSettings = updated.PasswordSettings;
    }

    if (updated.LoggingSettings != null)
    {
      _loggingSettings = updated.LoggingSettings;
    }
  }

  protected T GetLatestEvent<T>() where T : DomainEvent, new()
  {
    T? change = Changes.LastOrDefault(change => change is T) as T;
    if (change == null)
    {
      change = new();
      ApplyChange(change);
    }

    return change;
  }
}
