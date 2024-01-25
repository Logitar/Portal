using Logitar.EventSourcing;
using Logitar.Identity.Domain;
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
    get => _uniqueNameSettings ?? throw new InvalidOperationException($"The {nameof(UniqueNameSettings)} has not been initialized yet.");
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
    get => _passwordSettings ?? throw new InvalidOperationException($"The {nameof(PasswordSettings)} has not been initialized yet.");
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

  public ConfigurationAggregate(AggregateId id) : base(id)
  {
  }

  public static ConfigurationAggregate Initialize(LocaleUnit defaultLocale, ActorId actorId)
  {
    ConfigurationId id = new();
    ConfigurationAggregate configuration = new(id.AggregateId);
    configuration.Raise(new ConfigurationInitializedEvent(actorId, defaultLocale));
    return configuration;
  }
  protected virtual void Apply(ConfigurationInitializedEvent @event)
  {
    _defaultLocale = @event.DefaultLocale;
    _secret = @event.Secret;

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _requireUniqueEmail = @event.RequireUniqueEmail;
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
  }
}
