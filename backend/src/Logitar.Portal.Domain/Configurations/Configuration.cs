using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Domain.Configurations;

public class Configuration : AggregateRoot
{
  private ConfigurationUpdated _updated = new();

  public new ConfigurationId Id => new(base.Id);

  private Locale? _defaultLocale = null;
  public Locale? DefaultLocale
  {
    get => _defaultLocale;
    set
    {
      if (_defaultLocale != value)
      {
        _defaultLocale = value;
        _updated.DefaultLocale = new Change<Locale>(value);
      }
    }
  }
  private JwtSecret? _secret = null;
  public JwtSecret Secret
  {
    get => _secret ?? throw new InvalidOperationException($"The {nameof(Secret)} has not been initialized yet.");
    set
    {
      if (_secret != value)
      {
        _secret = value;
        _updated.Secret = value;
      }
    }
  }

  private ReadOnlyUniqueNameSettings? _uniqueNameSettings = null;
  public ReadOnlyUniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings ?? throw new InvalidOperationException($"The {nameof(UniqueNameSettings)} have not been initialized yet.");
    set
    {
      if (_uniqueNameSettings != value)
      {
        _uniqueNameSettings = value;
        _updated.UniqueNameSettings = value;
      }
    }
  }
  private ReadOnlyPasswordSettings? _passwordSettings = null;
  public ReadOnlyPasswordSettings PasswordSettings
  {
    get => _passwordSettings ?? throw new InvalidOperationException($"The {nameof(PasswordSettings)} have not been initialized yet.");
    set
    {
      if (_passwordSettings != value)
      {
        _passwordSettings = value;
        _updated.PasswordSettings = value;
      }
    }
  }
  private bool _requireUniqueEmail = false;
  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail;
    set
    {
      if (_requireUniqueEmail != value)
      {
        _requireUniqueEmail = value;
        _updated.RequireUniqueEmail = value;
      }
    }
  }

  private ReadOnlyLoggingSettings? _loggingSettings = null;
  public ReadOnlyLoggingSettings LoggingSettings
  {
    get => _loggingSettings ?? throw new InvalidOperationException($"The {nameof(LoggingSettings)} have not been initialized yet.");
    set
    {
      if (_loggingSettings != value)
      {
        _loggingSettings = value;
        _updated.LoggingSettings = value;
      }
    }
  }

  public Configuration() : base()
  {
  }

  private Configuration(ConfigurationId id) : base(id.StreamId)
  {
  }

  public static Configuration Initialize(ActorId? actorId = null)
  {
    ConfigurationId id = new();
    Configuration configuration = new(id);

    Locale? defaultLocale = null;
    JwtSecret secret = JwtSecret.Generate();
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    ReadOnlyPasswordSettings passordSettings = new();
    bool requireUniqueEmail = true;
    ReadOnlyLoggingSettings loggingSettings = new();
    configuration.Raise(new ConfigurationInitialized(defaultLocale, secret, uniqueNameSettings, passordSettings, requireUniqueEmail, loggingSettings), actorId);

    return configuration;
  }
  protected virtual void Handle(ConfigurationInitialized @event)
  {
    _defaultLocale = @event.DefaultLocale;
    _secret = @event.Secret;

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _requireUniqueEmail = @event.RequireUniqueEmail;

    _loggingSettings = @event.LoggingSettings;
  }

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(ConfigurationUpdated @event)
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
