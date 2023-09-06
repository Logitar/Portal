using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Roles.Events;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Roles;
public class RoleAggregate : AggregateRoot
{
  private readonly Dictionary<string, string> _customAttributes = new();

  private string? _displayName = null;
  private string? _description = null;

  public RoleAggregate(AggregateId id) : base(id)
  {
  }

  public RoleAggregate(IUniqueNameSettings uniqueNameSettings, string uniqueName, string? tenantId = null, ActorId actorId = default, AggregateId? id = null)
    : base(id)
  {
    uniqueName = uniqueName.Trim();
    new UniqueNameValidator(uniqueNameSettings, nameof(UniqueName)).ValidateAndThrow(uniqueName);

    tenantId = tenantId?.CleanTrim();
    if (tenantId != null)
    {
      new TenantIdValidator(nameof(TenantId)).ValidateAndThrow(tenantId);
    }

    ApplyChange(new RoleCreatedEvent(actorId)
    {
      TenantId = tenantId,
      UniqueName = uniqueName
    });
  }
  protected virtual void Apply(RoleCreatedEvent created)
  {
    TenantId = created.TenantId;

    UniqueName = created.UniqueName;
  }

  public string? TenantId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
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
        RoleUpdatedEvent updated = GetLatestEvent<RoleUpdatedEvent>();
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
        RoleUpdatedEvent updated = GetLatestEvent<RoleUpdatedEvent>();
        updated.Description = new Modification<string>(value);
        _description = value;
      }
    }
  }

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  public void Delete(ActorId actorId = default) => ApplyChange(new RoleDeletedEvent(actorId));

  public void RemoveCustomAttribute(string key)
  {
    key = key.Trim();
    if (_customAttributes.ContainsKey(key))
    {
      RoleUpdatedEvent updated = GetLatestEvent<RoleUpdatedEvent>();
      updated.CustomAttributes[key] = null;
      _customAttributes.Remove(key);
    }
  }

  public void SetCustomAttribute(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    new CustomAttributeValidator().ValidateAndThrow(key, value);

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
    {
      RoleUpdatedEvent updated = GetLatestEvent<RoleUpdatedEvent>();
      updated.CustomAttributes[key] = value;
      _customAttributes[key] = value;
    }
  }

  public void SetUniqueName(IUniqueNameSettings uniqueNameSettings, string uniqueName)
  {
    uniqueName = uniqueName.Trim();
    new UniqueNameValidator(uniqueNameSettings, nameof(UniqueName)).ValidateAndThrow(uniqueName);

    if (uniqueName != UniqueName)
    {
      RoleUpdatedEvent updated = GetLatestEvent<RoleUpdatedEvent>();
      updated.UniqueName = uniqueName;
      UniqueName = uniqueName;
    }
  }

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is RoleUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }

  protected virtual void Apply(RoleUpdatedEvent updated)
  {
    if (updated.UniqueName != null)
    {
      UniqueName = updated.UniqueName;
    }
    if (updated.DisplayName != null)
    {
      _displayName = updated.DisplayName.Value;
    }
    if (updated.Description != null)
    {
      _description = updated.Description.Value;
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

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
