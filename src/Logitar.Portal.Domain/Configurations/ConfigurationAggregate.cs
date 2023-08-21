using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  private const int SecretLength = 256 / 8;

  public static readonly AggregateId UniqueId = new("CONFIGURATION");

  private Locale _defaultLocale = new("en");
  private string _secret = string.Empty;

  private ReadOnlyUniqueNameSettings _uniqueNameSettings = new();
  private ReadOnlyPasswordSettings _passwordSettings = new();

  private ReadOnlyLoggingSettings _loggingSettings = new();

  public ConfigurationAggregate(AggregateId id) : base(id)
  {
  }

  public ConfigurationAggregate(Locale defaultLocale, string? secret = null,
    ReadOnlyUniqueNameSettings? uniqueNameSettings = null,
    ReadOnlyPasswordSettings? passwordSettings = null,
    ReadOnlyLoggingSettings? loggingSettings = null, ActorId actorId = default) : base(UniqueId)
  {
    if (secret == null)
    {
      secret = RandomStringGenerator.GetString(SecretLength);
    }
    else
    {
      secret = secret.Trim();
      new SecretValidator(nameof(Secret)).ValidateAndThrow(secret);
    }

    ApplyChange(new ConfigurationInitializedEvent
    {
      ActorId = actorId,
      DefaultLocale = defaultLocale,
      Secret = secret ?? RandomStringGenerator.GetString(),
      UniqueNameSettings = uniqueNameSettings ?? new(),
      PasswordSettings = passwordSettings ?? new(),
      LoggingSettings = loggingSettings ?? new()
    });
  }
  protected virtual void Apply(ConfigurationInitializedEvent initialized)
  {
    DefaultLocale = initialized.DefaultLocale;
    Secret = initialized.Secret;

    UniqueNameSettings = initialized.UniqueNameSettings;
    PasswordSettings = initialized.PasswordSettings;

    LoggingSettings = initialized.LoggingSettings;
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

  public IUserSettings UserSettings => new UserSettings
  {
    RequireUniqueEmail = false,
    RequireConfirmedAccount = false,
    UniqueNameSettings = UniqueNameSettings,
    PasswordSettings = PasswordSettings
  };

  public void Update(ActorId actorId = default)
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

  protected virtual T GetLatestEvent<T>() where T : DomainEvent, new()
  {
    T? change = Changes.Last(change => change is T) as T;
    if (change == null)
    {
      change = new();
      ApplyChange(change);
    }

    return change;
  }
}
