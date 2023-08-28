using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  private const int SecretLength = 256 / 8;

  public static readonly AggregateId UniqueId = new("CONFIGURATION");

  private Locale _defaultLocale = Locale.Default;
  private string _secret = string.Empty;

  private ReadOnlyUniqueNameSettings _uniqueNameSettings = new();
  private ReadOnlyPasswordSettings _passwordSettings = new();

  private ReadOnlyLoggingSettings _loggingSettings = new();

  public ConfigurationAggregate(AggregateId id) : base(id)
  {
  }

  public ConfigurationAggregate(Locale defaultLocale, ActorId actorId = default) : base(UniqueId)
  {
    ApplyChange(new ConfigurationInitializedEvent(actorId)
    {
      DefaultLocale = defaultLocale,
      Secret = RandomStringGenerator.GetString(SecretLength)
    });
  }
  protected virtual void Apply(ConfigurationInitializedEvent initialized)
  {
    _defaultLocale = initialized.DefaultLocale;
    _secret = initialized.Secret;

    _uniqueNameSettings = initialized.UniqueNameSettings;
    _passwordSettings = initialized.PasswordSettings;

    _loggingSettings = initialized.LoggingSettings;
  }

  public Locale DefaultLocale
  {
    get => _defaultLocale;
    set
    {
      if (value != _defaultLocale)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.DefaultLocale = value;
        _defaultLocale = value;
      }
    }
  }
  public string Secret
  {
    get => _secret;
    set
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        value = RandomStringGenerator.GetString(SecretLength);
      }
      else
      {
        value = value.Trim();
        new SecretValidator(nameof(Secret)).ValidateAndThrow(value);
      }

      if (value != _secret)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.Secret = value;
        _secret = value;
      }
    }
  }

  public ReadOnlyUniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings;
    set
    {
      if (value != _uniqueNameSettings)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.UniqueNameSettings = value;
        _uniqueNameSettings = value;
      }
    }
  }
  public ReadOnlyPasswordSettings PasswordSettings
  {
    get => _passwordSettings;
    set
    {
      if (value != _passwordSettings)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.PasswordSettings = value;
        _passwordSettings = value;
      }
    }
  }

  public ReadOnlyLoggingSettings LoggingSettings
  {
    get => _loggingSettings;
    set
    {
      if (value != _loggingSettings)
      {
        ConfigurationUpdatedEvent updated = GetLatestEvent<ConfigurationUpdatedEvent>();
        updated.LoggingSettings = value;
        _loggingSettings = value;
      }
    }
  }

  public IUserSettings UserSettings => new ReadOnlyUserSettings(requireUniqueEmail: false,
    requireConfirmedAccout: false, UniqueNameSettings, PasswordSettings);

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is ConfigurationUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
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

  protected virtual T GetLatestEvent<T>() where T : DomainEvent, new()
  {
    T? updated = Changes.SingleOrDefault(change => change is T) as T;
    if (updated == null)
    {
      updated = new();
      ApplyChange(updated);
    }

    return updated;
  }
}
