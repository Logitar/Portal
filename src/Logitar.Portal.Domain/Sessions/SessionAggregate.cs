using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Sessions;

public class SessionAggregate : AggregateRoot
{
  public const int SecretLength = 256 / 8;

  private readonly Dictionary<string, string> _customAttributes = new();

  private Password? _secret = null;

  public SessionAggregate(AggregateId id) : base(id)
  {
  }

  public SessionAggregate(UserAggregate user, Password? secret = null, ActorId actorId = default, AggregateId? id = null)
    : base(id)
  {
    ApplyChange(new SessionCreatedEvent(actorId)
    {
      UserId = user.Id,
      Secret = secret
    });
  }
  protected virtual void Apply(SessionCreatedEvent created)
  {
    _secret = created.Secret;

    UserId = created.UserId;

    IsActive = true;
  }

  public AggregateId UserId { get; private set; }

  public bool IsPersistent => _secret != null;

  public bool IsActive { get; private set; }

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  public void RemoveCustomAttribute(string key)
  {
    key = key.Trim();
    if (_customAttributes.ContainsKey(key))
    {
      SessionUpdatedEvent updated = GetLatestEvent<SessionUpdatedEvent>();
      updated.CustomAttributes[key] = null;
      _customAttributes.Remove(key);
    }
  }

  public void Renew(string secret, Password? newSecret = null, ActorId? actorId = null)
  {
    if (!IsActive)
    {
      throw new SessionIsNotActiveException(this);
    }
    else if (_secret?.IsMatch(secret) != true)
    {
      throw new IncorrectSessionSecretException(this, secret);
    }

    actorId ??= new(UserId.Value);

    ApplyChange(new SessionRenewedEvent(actorId.Value)
    {
      Secret = newSecret
    });
  }
  protected virtual void Apply(SessionRenewedEvent renewed) => _secret = renewed.Secret;

  public void SetCustomAttribute(string key, string value)
  {
    key = key.Trim();
    value = value.Trim();
    new CustomAttributeValidator().ValidateAndThrow(key, value);

    if (!_customAttributes.TryGetValue(key, out string? existingValue) || value != existingValue)
    {
      SessionUpdatedEvent updated = GetLatestEvent<SessionUpdatedEvent>();
      updated.CustomAttributes[key] = value;
      _customAttributes[key] = value;
    }
  }

  public void Update(ActorId actorId)
  {
    foreach (DomainEvent change in Changes)
    {
      if (change is SessionUpdatedEvent updated && updated.ActorId == default)
      {
        updated.ActorId = actorId;

        if (updated.Version == Version)
        {
          UpdatedBy = actorId;
        }
      }
    }
  }

  protected virtual void Apply(SessionUpdatedEvent updated)
  {
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
}
