using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Domain.Realms;

public class RealmAggregate : AggregateRoot
{
  private RealmUpdated _updated = new();

  public new RealmId Id => new(base.Id);

  private Slug? _uniqueSlug = null;
  public Slug UniqueSlug => _uniqueSlug ?? throw new InvalidOperationException($"The {nameof(UniqueSlug)} has not been initialized yet.");
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

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
  private Url? _url = null;
  public Url? Url
  {
    get => _url;
    set
    {
      if (_url != value)
      {
        _url = value;
        _updated.Url = new Change<Url>(value);
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
  private bool? _requireUniqueEmail;
  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail ?? throw new InvalidOperationException($"The {nameof(RequireUniqueEmail)} have not been initialized yet.");
    set
    {
      if (_requireUniqueEmail != value)
      {
        _requireUniqueEmail = value;
        _updated.RequireUniqueEmail = value;
      }
    }
  }

  private readonly Dictionary<Identifier, string> _customAttributes = [];
  public IReadOnlyDictionary<Identifier, string> CustomAttributes => _customAttributes.AsReadOnly();

  public RealmAggregate() : base()
  {
  }

  public RealmAggregate(Slug uniqueSlug, ActorId actorId = default, RealmId? id = null) : base((id ?? RealmId.NewId()).StreamId)
  {
    JwtSecret secret = JwtSecret.Generate();
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    ReadOnlyPasswordSettings passwordSettings = new();
    bool requireUniqueEmail = true;
    Raise(new RealmCreated(uniqueSlug, secret, uniqueNameSettings, passwordSettings, requireUniqueEmail), actorId);
  }
  protected virtual void Apply(RealmCreated @event)
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
      Raise(new RealmDeleted(), actorId);
    }
  }

  public void RemoveCustomAttribute(Identifier key)
  {
    if (_customAttributes.Remove(key))
    {
      _updated.CustomAttributes[key] = null;
    }
  }

  public void SetCustomAttribute(Identifier key, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      RemoveCustomAttribute(key);
    }

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || existingValue != value)
    {
      _updated.CustomAttributes[key] = value;
      _customAttributes[key] = value;
    }
  }

  public void SetUniqueSlug(Slug uniqueSlug, ActorId actorId = default)
  {
    if (_uniqueSlug != uniqueSlug)
    {
      Raise(new RealmUniqueSlugChanged(uniqueSlug), actorId);
    }
  }
  protected virtual void Apply(RealmUniqueSlugChanged @event)
  {
    _uniqueSlug = @event.UniqueSlug;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Apply(RealmUpdated @event)
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

    foreach (KeyValuePair<Identifier, string?> customAttribute in @event.CustomAttributes)
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
