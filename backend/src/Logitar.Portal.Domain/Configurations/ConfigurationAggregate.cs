using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Domain.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  private ConfigurationUpdatedEvent _updatedEvent = new();

  private LocaleUnit? _defaultLocale = null;
  public LocaleUnit? DefaultLocale
  {
    get => _defaultLocale;
    set
    {
      if (value != _defaultLocale)
      {
        _defaultLocale = value;
        _updatedEvent.DefaultLocale = new Modification<LocaleUnit>(value);
      }
    }
  }
  private JwtSecretUnit? _secret = null;
  public JwtSecretUnit Secret
  {
    get => _secret ?? throw new InvalidOperationException($"The {nameof(Secret)} has not been initialized yet.");
    set
    {
      if (value != _secret)
      {
        _secret = value;
        _updatedEvent.Secret = value;
      }
    }
  }

  private ReadOnlyUniqueNameSettings? _uniqueNameSettings = null;
  public ReadOnlyUniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings ?? throw new InvalidOperationException($"The {nameof(UniqueNameSettings)} have not been initialized yet.");
    set
    {
      if (value != _uniqueNameSettings)
      {
        _uniqueNameSettings = value;
        _updatedEvent.UniqueNameSettings = value;
      }
    }
  }
  private ReadOnlyPasswordSettings? _passwordSettings = null;
  public ReadOnlyPasswordSettings PasswordSettings
  {
    get => _passwordSettings ?? throw new InvalidOperationException($"The {nameof(PasswordSettings)} have not been initialized yet.");
    set
    {
      if (value != _passwordSettings)
      {
        _passwordSettings = value;
        _updatedEvent.PasswordSettings = value;
      }
    }
  }
  private bool _requireUniqueEmail = false;
  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail;
    set
    {
      if (value != _requireUniqueEmail)
      {
        _requireUniqueEmail = value;
        _updatedEvent.RequireUniqueEmail = value;
      }
    }
  }

  private ReadOnlyLoggingSettings? _loggingSettings = null;
  public ReadOnlyLoggingSettings LoggingSettings
  {
    get => _loggingSettings ?? throw new InvalidOperationException($"The {nameof(LoggingSettings)} have not been initialized yet.");
    set
    {
      if (value != _loggingSettings)
      {
        _loggingSettings = value;
        _updatedEvent.LoggingSettings = value;
      }
    }
  }

  public ConfigurationAggregate(AggregateId id) : base(id)
  {
  }

  public static ConfigurationAggregate Initialize(ActorId actorId)
  {
    ConfigurationId id = new();
    ConfigurationAggregate configuration = new(id.AggregateId);

    JwtSecretUnit secret = JwtSecretUnit.Generate();
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    ReadOnlyPasswordSettings passordSettings = new();
    bool requireUniqueEmail = true;
    ReadOnlyLoggingSettings loggingSettings = new();
    configuration.Raise(new ConfigurationInitializedEvent(actorId, defaultLocale: null, secret, uniqueNameSettings, passordSettings, requireUniqueEmail, loggingSettings));

    return configuration;
  }
  protected virtual void Apply(ConfigurationInitializedEvent @event)
  {
    _defaultLocale = @event.DefaultLocale;
    _secret = @event.Secret;

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _requireUniqueEmail = @event.RequireUniqueEmail;

    _loggingSettings = @event.LoggingSettings;
  }

  public void Update(ActorId actorId)
  {
    if (_updatedEvent.HasChanges)
    {
      _updatedEvent.ActorId = actorId;
      Raise(_updatedEvent);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(ConfigurationUpdatedEvent @event)
  {
    if (@event.DefaultLocale != null)
    {
      _defaultLocale = @event.DefaultLocale.Value;
    }
    if (@event.Secret != null)
    {
      _secret = @event.Secret;
    }

    if (@event.UniqueNameSettings != null)
    {
      _uniqueNameSettings = @event.UniqueNameSettings;
    }
    if (@event.PasswordSettings != null)
    {
      _passwordSettings = @event.PasswordSettings;
    }
    if (@event.RequireUniqueEmail.HasValue)
    {
      _requireUniqueEmail = @event.RequireUniqueEmail.Value;
    }

    if (@event.LoggingSettings != null)
    {
      _loggingSettings = @event.LoggingSettings;
    }
  }
}
