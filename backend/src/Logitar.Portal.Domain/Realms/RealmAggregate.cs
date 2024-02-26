using Logitar.EventSourcing;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Domain.Realms;

public class RealmAggregate : AggregateRoot
{
  private RealmUpdatedEvent _updatedEvent = new();

  public new RealmId Id => new(base.Id);

  private UniqueSlugUnit? _uniqueSlug = null;
  public UniqueSlugUnit UniqueSlug => _uniqueSlug ?? throw new InvalidOperationException($"The {nameof(UniqueSlug)} has not been initialized yet.");
  private DisplayNameUnit? _displayName = null;
  public DisplayNameUnit? DisplayName
  {
    get => _displayName;
    set
    {
      if (value != _displayName)
      {
        _displayName = value;
        _updatedEvent.DisplayName = new Modification<DisplayNameUnit>(value);
      }
    }
  }
  private DescriptionUnit? _description = null;
  public DescriptionUnit? Description
  {
    get => _description;
    set
    {
      if (value != _description)
      {
        _description = value;
        _updatedEvent.Description = new Modification<DescriptionUnit>(value);
      }
    }
  }

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
  private UrlUnit? _url = null;
  public UrlUnit? Url
  {
    get => _url;
    set
    {
      if (value != _url)
      {
        _url = value;
        _updatedEvent.Url = new Modification<UrlUnit>(value);
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
  private bool? _requireUniqueEmail;
  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail ?? throw new InvalidOperationException($"The {nameof(RequireUniqueEmail)} have not been initialized yet.");
    set
    {
      if (value != _requireUniqueEmail)
      {
        _requireUniqueEmail = value;
        _updatedEvent.RequireUniqueEmail = value;
      }
    }
  }

  private readonly Dictionary<string, string> _customAttributes = [];
  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  public RealmAggregate(AggregateId id) : base(id)
  {
  }

  public RealmAggregate(UniqueSlugUnit uniqueSlug, ActorId actorId = default, RealmId? id = null) : base((id ?? RealmId.NewId()).AggregateId)
  {
    JwtSecretUnit secret = JwtSecretUnit.Generate();
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    ReadOnlyPasswordSettings passwordSettings = new();
    bool requireUniqueEmail = true;
    Raise(new RealmCreatedEvent(actorId, uniqueSlug, secret, uniqueNameSettings, passwordSettings, requireUniqueEmail));
  }
  protected virtual void Apply(RealmCreatedEvent @event)
  {
    _uniqueSlug = @event.UniqueSlug;
    _secret = @event.Secret;
    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _requireUniqueEmail = @event.RequireUniqueEmail;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new RealmDeletedEvent(actorId));
    }
  }

  public void RemoveCustomAttribute(string key)
  {
    key = key.Trim();

    if (_customAttributes.ContainsKey(key))
    {
      _updatedEvent.CustomAttributes[key] = null;
      _customAttributes.Remove(key);
    }
  }

  private readonly CustomAttributeValidator _customAttributeValidator = new();
  public void SetCustomAttribute(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    _customAttributeValidator.ValidateAndThrow(key, value);

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
    {
      _updatedEvent.CustomAttributes[key] = value;
      _customAttributes[key] = value;
    }
  }

  public void SetUniqueSlug(UniqueSlugUnit uniqueSlug, ActorId actorId = default)
  {
    Raise(new RealmUniqueSlugChangedEvent(actorId, uniqueSlug));
  }
  protected virtual void Apply(RealmUniqueSlugChangedEvent @event)
  {
    _uniqueSlug = @event.UniqueSlug;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      _updatedEvent.ActorId = actorId;
      Raise(_updatedEvent);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(RealmUpdatedEvent @event)
  {
    if (@event.DisplayName != null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description != null)
    {
      _description = @event.Description.Value;
    }

    if (@event.DefaultLocale != null)
    {
      _defaultLocale = @event.DefaultLocale.Value;
    }
    if (@event.Secret != null)
    {
      _secret = @event.Secret;
    }
    if (@event.Url != null)
    {
      _url = @event.Url.Value;
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

    foreach (KeyValuePair<string, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        _customAttributes.Remove(customAttribute.Key);
      }
      else
      {
        _customAttributes[customAttribute.Key] = customAttribute.Value;
      }
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueSlug.Value} | {base.ToString()}";
}
