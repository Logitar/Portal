using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Domain.Realms.Validators;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Realms;

public class RealmAggregate : AggregateRoot
{
  private const int SecretLength = 256 / 8;

  private readonly Dictionary<string, ReadOnlyClaimMapping> _claimMappings = new();
  private readonly Dictionary<string, string> _customAttributes = new();

  private string _uniqueSlug = string.Empty;
  private string? _displayName = null;
  private string? _description = null;

  private Locale? _defaultLocale = null;
  private string _secret = string.Empty;
  private Uri? _url = null;

  private bool _requireUniqueEmail = false;
  private bool _requireConfirmedAccount = false;

  private ReadOnlyUniqueNameSettings _uniqueNameSettings = new();
  private ReadOnlyPasswordSettings _passwordSettings = new();

  public RealmAggregate(AggregateId id) : base(id)
  {
  }

  public RealmAggregate(string uniqueSlug, string? secret = null, bool requireUniqueEmail = false, bool requireConfirmedAccount = false,
    ReadOnlyUniqueNameSettings? uniqueNameSettings = null, ReadOnlyPasswordSettings? passwordSettings = null, ActorId actorId = default, AggregateId? id = null)
      : base(id)
  {
    uniqueSlug = uniqueSlug.Trim();
    new UniqueSlugValidator(nameof(UniqueSlug)).ValidateAndThrow(uniqueSlug);

    if (string.IsNullOrWhiteSpace(secret))
    {
      secret = RandomStringGenerator.GetString(SecretLength);
    }
    else
    {
      secret = secret.Trim();
      new SecretValidator(nameof(Secret)).ValidateAndThrow(secret);
    }

    ApplyChange(new RealmCreatedEvent(actorId)
    {
      UniqueSlug = uniqueSlug,
      Secret = secret,
      RequireUniqueEmail = requireUniqueEmail,
      RequireConfirmedAccount = requireConfirmedAccount,
      UniqueNameSettings = uniqueNameSettings ?? new(),
      PasswordSettings = passwordSettings ?? new()
    });
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
      value = value.Trim();
      new UniqueSlugValidator(nameof(UniqueSlug)).ValidateAndThrow(value);

      if (value != _uniqueSlug)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.UniqueSlug = value;
        _uniqueSlug = value;
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
        updated.DisplayName = new Modification<string>(value);
        _displayName = value;
      }
    }
  }
  public string? Description
  {
    get => _description;
    set
    {
      value = value?.CleanTrim();

      if (value != _description)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.Description = new Modification<string>(value);
        _description = value;
      }
    }
  }

  public Locale? DefaultLocale
  {
    get => _defaultLocale;
    set
    {
      if (value != _defaultLocale)
      {
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.DefaultLocale = new Modification<Locale>(value);
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
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.Secret = value;
        _secret = value;
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
        updated.Url = new Modification<Uri>(value);
        _url = value;
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
        _requireUniqueEmail = value;
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
        _requireConfirmedAccount = value;
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
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
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
        RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
        updated.PasswordSettings = value;
        _passwordSettings = value;
      }
    }
  }

  public IReadOnlyDictionary<string, ReadOnlyClaimMapping> ClaimMappings => _claimMappings.AsReadOnly();

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  public AggregateId? PasswordRecoverySenderId { get; private set; }

  public IUserSettings UserSettings => new ReadOnlyUserSettings(RequireUniqueEmail,
    RequireConfirmedAccount, UniqueNameSettings, PasswordSettings);

  public void Delete(ActorId actorId = default) => ApplyChange(new RealmDeletedEvent(actorId));

  public void RemoveClaimMapping(string key)
  {
    key = key.Trim();
    if (_claimMappings.ContainsKey(key))
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.ClaimMappings[key] = null;
      _claimMappings.Remove(key);
    }
  }

  public void RemoveCustomAttribute(string key)
  {
    key = key.Trim();
    if (_customAttributes.ContainsKey(key))
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.CustomAttributes[key] = null;
      _customAttributes.Remove(key);
    }
  }

  public void RemovePasswordRecoverySender()
  {
    if (PasswordRecoverySenderId.HasValue)
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.PasswordRecoverySenderId = new Modification<AggregateId?>(null);
      PasswordRecoverySenderId = null;
    }
  }

  public void SetClaimMapping(string key, ReadOnlyClaimMapping claimMapping)
  {
    key = key.Trim();
    new CustomAttributeKeyValidator("Key").ValidateAndThrow(key);

    if (!_claimMappings.TryGetValue(key, out ReadOnlyClaimMapping? existingClaimMapping) || claimMapping != existingClaimMapping)
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.ClaimMappings[key] = claimMapping;
      _claimMappings[key] = claimMapping;
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
      _customAttributes[key] = value;
    }
  }

  public void SetPasswordRecoverySender(SenderAggregate sender, string propertyName)
  {
    if (sender.TenantId != Id.Value)
    {
      throw new SenderNotInRealmException(sender, this, propertyName ?? nameof(PasswordRecoverySenderId));
    }

    if (sender.Id != PasswordRecoverySenderId)
    {
      RealmUpdatedEvent updated = GetLatestEvent<RealmUpdatedEvent>();
      updated.PasswordRecoverySenderId = new Modification<AggregateId?>(sender?.Id);
      PasswordRecoverySenderId = sender?.Id;
    }
  }

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is RealmUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
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

    if (updated.PasswordRecoverySenderId != null)
    {
      PasswordRecoverySenderId = updated.PasswordRecoverySenderId.Value;
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

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
