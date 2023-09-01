using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.Domain.ApiKeys.Validators;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.ApiKeys;

public class ApiKeyAggregate : AggregateRoot
{
  public const int SecretLength = 32;

  private readonly Dictionary<string, string> _customAttributes = new();
  private readonly HashSet<AggregateId> _roles = new();

  private Password? _secret = null;

  private string _title = string.Empty;
  private string? _description = null;
  private DateTime? _expiresOn = null;

  public ApiKeyAggregate(AggregateId id) : base(id)
  {
  }

  public ApiKeyAggregate(string title, Password secret, string? tenantId = null, ActorId actorId = default, AggregateId? id = null)
    : base(id)
  {
    title = title.Trim();
    new TitleValidator(nameof(Title)).ValidateAndThrow(title);

    tenantId = tenantId?.CleanTrim();
    if (tenantId != null)
    {
      new TenantIdValidator(nameof(TenantId)).ValidateAndThrow(tenantId);
    }

    ApplyChange(new ApiKeyCreatedEvent(actorId)
    {
      Secret = secret,
      TenantId = tenantId,
      Title = title
    });
  }
  protected virtual void Apply(ApiKeyCreatedEvent created)
  {
    _secret = created.Secret;

    TenantId = created.TenantId;

    _title = created.Title;
  }

  public string? TenantId { get; private set; }

  public string Title
  {
    get => _title;
    set
    {
      value = value.Trim();
      new TitleValidator(nameof(Title)).ValidateAndThrow(value);

      if (value != _title)
      {
        ApiKeyUpdatedEvent updated = GetLatestEvent<ApiKeyUpdatedEvent>();
        updated.Title = value;
        _title = value;
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
        ApiKeyUpdatedEvent updated = GetLatestEvent<ApiKeyUpdatedEvent>();
        updated.Description = new Modification<string>(value);
        _description = value;
      }
    }
  }
  public DateTime? ExpiresOn
  {
    get => _expiresOn;
    set
    {
      if (value.HasValue)
      {
        new ExpirationValidator(nameof(ExpiresOn)).ValidateAndThrow(value.Value);
      }

      if (_expiresOn.HasValue && value == null || _expiresOn < value)
      {
        throw new CannotPostponeExpirationException(this, value, nameof(ExpiresOn));
      }

      if (value != _expiresOn)
      {
        ApiKeyUpdatedEvent updated = GetLatestEvent<ApiKeyUpdatedEvent>();
        updated.ExpiresOn = value;
        _expiresOn = value;
      }
    }
  }

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  public IImmutableSet<AggregateId> Roles => ImmutableHashSet.Create(_roles.ToArray());

  public void AddRole(RoleAggregate role)
  {
    AggregateId roleId = role.Id;
    if (!_roles.Contains(roleId))
    {
      ApiKeyUpdatedEvent updated = GetLatestEvent<ApiKeyUpdatedEvent>();
      updated.Roles[roleId.Value] = CollectionAction.Add;
      _roles.Add(roleId);
    }
  }

  public void Authenticate(string secret, ActorId? actorId = null)
  {
    if (IsExpired())
    {
      throw new ApiKeyIsExpiredException(this);
    }
    else if (_secret?.IsMatch(secret) != true)
    {
      throw new IncorrectApiKeySecretException(this, secret);
    }

    actorId ??= new(Id.Value);

    ApplyChange(new ApiKeyAuthenticatedEvent(actorId.Value));
  }

  public void Delete(ActorId actorId = default) => ApplyChange(new ApiKeyDeletedEvent(actorId));

  public bool IsExpired(DateTime? moment = null) => ExpiresOn.HasValue && ExpiresOn.Value <= (moment ?? DateTime.Now);

  public void RemoveCustomAttribute(string key)
  {
    key = key.Trim();
    if (_customAttributes.ContainsKey(key))
    {
      ApiKeyUpdatedEvent updated = GetLatestEvent<ApiKeyUpdatedEvent>();
      updated.CustomAttributes[key] = null;
      _customAttributes.Remove(key);
    }
  }

  public void RemoveRole(RoleAggregate role) => RemoveRole(role.Id);
  public void RemoveRole(AggregateId roleId)
  {
    if (_roles.Contains(roleId))
    {
      ApiKeyUpdatedEvent updated = GetLatestEvent<ApiKeyUpdatedEvent>();
      updated.Roles[roleId.Value] = CollectionAction.Remove;
      _roles.Remove(roleId);
    }
  }

  public void SetCustomAttribute(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    new CustomAttributeValidator().ValidateAndThrow(key, value);

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
    {
      ApiKeyUpdatedEvent updated = GetLatestEvent<ApiKeyUpdatedEvent>();
      updated.CustomAttributes[key] = value;
      _customAttributes[key] = value;
    }
  }

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is ApiKeyUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }

  protected virtual void Apply(ApiKeyUpdatedEvent updated)
  {
    if (updated.Title != null)
    {
      _title = updated.Title;
    }
    if (updated.Description != null)
    {
      _description = updated.Description.Value;
    }
    if (updated.ExpiresOn.HasValue)
    {
      _expiresOn = updated.ExpiresOn.Value;
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

    foreach (KeyValuePair<string, CollectionAction> role in updated.Roles)
    {
      AggregateId roleId = new(role.Key);

      switch (role.Value)
      {
        case CollectionAction.Add:
          _roles.Add(roleId);
          break;
        case CollectionAction.Remove:
          _roles.Remove(roleId);
          break;
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

  public override string ToString() => $"{Title} | {base.ToString()}";
}
