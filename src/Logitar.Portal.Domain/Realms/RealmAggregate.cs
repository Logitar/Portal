using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Roles.Validators;
using Logitar.Identity.Domain.Validators;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Domain.Realms.Validators;
using Logitar.Portal.Domain.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Realms;

public class RealmAggregate : AggregateRoot
{
  private readonly Dictionary<string, ReadOnlyClaimMapping> _claimMappings = new();
  private readonly Dictionary<string, string> _customAttributes = new();

  private string _uniqueSlug = string.Empty;
  private string? _displayName = null;
  private string? _description = null;

  private CultureInfo? _defaultLocale = null;
  private string _secret = string.Empty;
  private Uri? _url = null;

  private bool _requireUniqueEmail = false;
  private bool _requireConfirmedAccount = false;

  private ReadOnlyUniqueNameSettings _uniqueNameSettings = new();
  private ReadOnlyPasswordSettings _passwordSettings = new();

  public RealmAggregate(AggregateId id) : base(id)
  {
  }

  public RealmAggregate(string uniqueSlug, ActorId actorId = default) : base()
  {
    RealmCreatedEvent created = new()
    {
      ActorId = actorId,
      UniqueSlug = uniqueSlug,
      Secret = RandomStringGenerator.GetString()
    };
    new RealmCreatedValidator().ValidateAndThrow(created);

    ApplyChange(created);
  }
  protected virtual void Apply(RealmCreatedEvent created)
  {
    _uniqueSlug = created.UniqueSlug;

    _secret = created.Secret;

    _requireUniqueEmail = created.RequireUniqueEmail;
    _requireConfirmedAccount = created.RequireConfirmedAccount;

    _uniqueNameSettings = created.UniqueNameSettings;
    _passwordSettings = created.PasswordSettings;
  }

  public string UniqueSlug
  {
    get => _uniqueSlug;
    set
    {
      new UniqueSlugValidator(nameof(UniqueSlug)).ValidateAndThrow(value);

      if (value != _uniqueSlug)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.UniqueSlug = value;
        Apply(updated);
      }
    }
  }
  public string? DisplayName
  {
    get => _displayName;
    set
    {
      value = value?.CleanTrim();
      if (value != null)
      {
        new DisplayNameValidator(nameof(DisplayName)).ValidateAndThrow(value);
      }

      if (value != _displayName)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.DisplayName = new MayBe<string>(value);
        Apply(updated);
      }
    }
  }
  public string? Description
  {
    get => _description;
    set
    {
      value = value?.CleanTrim();

      if (value != _displayName)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.Description = new MayBe<string>(value);
        Apply(updated);
      }
    }
  }

  public CultureInfo? DefaultLocale
  {
    get => _defaultLocale;
    set
    {
      if (value != null)
      {
        new LocaleValidator(nameof(DefaultLocale)).ValidateAndThrow(value);
      }

      if (value != _defaultLocale)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.DefaultLocale = new MayBe<CultureInfo>(value);
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
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.Secret = value;
        Apply(updated);
      }
    }
  }
  public Uri? Url
  {
    get => _url;
    set
    {
      if (value != _url)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.Url = new MayBe<Uri>(value);
        Apply(updated);
      }
    }
  }

  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail;
    set
    {
      if (value != _requireUniqueEmail)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.RequireUniqueEmail = value;
        Apply(updated);
      }
    }
  }
  public bool RequireConfirmedAccount
  {
    get => _requireConfirmedAccount;
    set
    {
      if (value != _requireConfirmedAccount)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.RequireConfirmedAccount = value;
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
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
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
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.PasswordSettings = value;
        Apply(updated);
      }
    }
  }

  public IReadOnlyDictionary<string, ReadOnlyClaimMapping> ClaimMappings => _claimMappings.AsReadOnly();

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  public void Delete(ActorId actorId = default) => ApplyChange(new RealmDeletedEvent(actorId));

  public void RemoveClaimMapping(string key)
  {
    key = key.Trim();
    if (_claimMappings.ContainsKey(key))
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.ClaimMappings[key] = null;
      Apply(updated);
    }
  }

  public void RemoveCustomAttribute(string key)
  {
    key = key.Trim();
    if (_customAttributes.ContainsKey(key))
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.CustomAttributes[key] = null;
      Apply(updated);
    }
  }

  public void SetClaimMapping(string key, ReadOnlyClaimMapping claimMapping)
  {
    key = key.Trim();
    new ClaimMappingKeyValidator("Key").ValidateAndThrow(key);

    if (!_claimMappings.TryGetValue(key, out ReadOnlyClaimMapping? existingClaimMapping)
      || claimMapping != existingClaimMapping)
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.ClaimMappings[key] = claimMapping;
      Apply(updated);
    }
  }

  public void SetCustomAttribute(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    new CustomAttributeValidator().ValidateAndThrow(key, value);

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.CustomAttributes[key] = value;
      Apply(updated);
    }
  }

  public void Update(ActorId actorId = default)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is RealmUpdatedEvent)
      {
        change.ActorId = actorId;

        if (change.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }
  protected virtual void Apply(RealmUpdatedEvent updated)
  {
    if (updated.UniqueSlug != null)
    {
      _uniqueSlug = updated.UniqueSlug;
    }
    if (updated.DisplayName != null)
    {
      _displayName = updated.DisplayName.Value;
    }
    if (updated.Description != null)
    {
      _description = updated.Description.Value;
    }

    if (updated.DefaultLocale != null)
    {
      _defaultLocale = updated.DefaultLocale.Value;
    }
    if (updated.Secret != null)
    {
      _secret = updated.Secret;
    }
    if (updated.Url != null)
    {
      _url = updated.Url.Value;
    }

    if (updated.RequireUniqueEmail.HasValue)
    {
      _requireUniqueEmail = updated.RequireUniqueEmail.Value;
    }
    if (updated.RequireConfirmedAccount.HasValue)
    {
      _requireConfirmedAccount = updated.RequireConfirmedAccount.Value;
    }

    if (updated.UniqueNameSettings != null)
    {
      _uniqueNameSettings = updated.UniqueNameSettings;
    }
    if (updated.PasswordSettings != null)
    {
      _passwordSettings = updated.PasswordSettings;
    }

    foreach (KeyValuePair<string, ReadOnlyClaimMapping?> claimMapping in updated.ClaimMappings)
    {
      if (claimMapping.Value == null)
      {
        _claimMappings.Remove(claimMapping.Key);
      }
      else
      {
        _claimMappings[claimMapping.Key] = claimMapping.Value;
      }
    }

    foreach (KeyValuePair<string, string?> customAttribute in updated.CustomAttributes)
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

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
