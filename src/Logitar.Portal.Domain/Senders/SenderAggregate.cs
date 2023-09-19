using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.Domain.Senders.Validators;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Senders;
public class SenderAggregate : AggregateRoot
{
  private readonly Dictionary<string, string> _settings = new();

  private bool _isDefault = false;

  private string _emailAddress = string.Empty;
  private string? _displayName = null;
  private string? _description = null;

  public SenderAggregate(AggregateId id) : base(id)
  {
  }

  public SenderAggregate(string emailAddress, ProviderType provider, bool isDefault = false,
    string? tenantId = null, ActorId actorId = default, AggregateId? id = null)
      : base(id)
  {
    emailAddress = emailAddress.Trim();
    _ = new EmailAddress(emailAddress);

    tenantId = tenantId?.CleanTrim();
    if (tenantId != null)
    {
      new TenantIdValidator(nameof(TenantId)).ValidateAndThrow(tenantId);
    }

    ApplyChange(new SenderCreatedEvent(actorId)
    {
      TenantId = tenantId,
      IsDefault = isDefault,
      Provider = provider,
      EmailAddress = emailAddress
    });
  }
  protected virtual void Apply(SenderCreatedEvent created)
  {
    TenantId = created.TenantId;

    _isDefault = created.IsDefault;

    _emailAddress = created.EmailAddress;

    Provider = created.Provider;
  }

  public string? TenantId { get; private set; }

  public bool IsDefault
  {
    get => _isDefault;
    set
    {
      if (value != _isDefault)
      {
        SenderUpdatedEvent updated = GetLatestEvent<SenderUpdatedEvent>();
        updated.IsDefault = value;
        _isDefault = value;
      }
    }
  }

  public string EmailAddress
  {
    get => _emailAddress;
    set
    {
      value = value.Trim();
      _ = new EmailAddress(value);

      if (value != _emailAddress)
      {
        SenderUpdatedEvent updated = GetLatestEvent<SenderUpdatedEvent>();
        updated.EmailAddress = value;
        _emailAddress = value;
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
        SenderUpdatedEvent updated = GetLatestEvent<SenderUpdatedEvent>();
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
        SenderUpdatedEvent updated = GetLatestEvent<SenderUpdatedEvent>();
        updated.Description = new Modification<string>(value);
        _description = value;
      }
    }
  }

  public ProviderType Provider { get; private set; }
  public IReadOnlyDictionary<string, string> Settings => _settings.AsReadOnly();

  public void Delete(ActorId actorId = default) => ApplyChange(new SenderDeletedEvent(actorId));

  public void RemoveSetting(string key)
  {
    key = key.Trim();
    if (_settings.ContainsKey(key))
    {
      SenderUpdatedEvent updated = GetLatestEvent<SenderUpdatedEvent>();
      updated.Settings[key] = null;
      _settings.Remove(key);
    }
  }

  public void SetSetting(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    new SenderSettingValidator().ValidateAndThrow(key, value);

    if (!_settings.TryGetValue(key, out string? existingValue) || value != existingValue)
    {
      SenderUpdatedEvent updated = GetLatestEvent<SenderUpdatedEvent>();
      updated.Settings[key] = value;
      _settings[key] = value;
    }
  }

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is SenderUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }

  protected virtual void Apply(SenderUpdatedEvent updated)
  {
    if (updated.IsDefault.HasValue)
    {
      _isDefault = updated.IsDefault.Value;
    }

    if (updated.EmailAddress != null)
    {
      _emailAddress = updated.EmailAddress;
    }
    if (updated.DisplayName != null)
    {
      _displayName = updated.DisplayName.Value;
    }
    if (updated.Description != null)
    {
      _description = updated.Description.Value;
    }

    foreach (KeyValuePair<string, string?> setting in updated.Settings)
    {
      if (setting.Value == null)
      {
        _settings.Remove(setting.Key);
      }
      else
      {
        _settings[setting.Key] = setting.Value;
      }
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

  public override string ToString()
  {
    StringBuilder formatted = new();

    if (DisplayName == null)
    {
      formatted.Append(EmailAddress);
    }
    else
    {
      formatted.Append(DisplayName).Append(" <").Append(EmailAddress).Append('>');
    }

    return $"{formatted} | {base.ToString()}";
  }
}
