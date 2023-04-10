using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Security;
using Logitar.Portal.v2.Core.Sessions.Events;
using Logitar.Portal.v2.Core.Sessions.Validators;
using Logitar.Portal.v2.Core.Users;
using System.Security.Cryptography;

namespace Logitar.Portal.v2.Core.Sessions;

public class SessionAggregate : AggregateRoot
{
  private const int KeyLength = 32;

  private readonly Dictionary<string, string> _customAttributes = new();
  private Pbkdf2? _key = null;

  public SessionAggregate(AggregateId id) : base(id)
  {
  }

  public SessionAggregate(AggregateId userId, DateTime signedInOn, bool isPersistent = false,
    string? ipAddress = null, string? additionalInformation = null,
    Dictionary<string, string>? customAttributes = null) : base()
  {
    byte[]? bytes = null;
    Pbkdf2? key = null;
    if (isPersistent)
    {
      bytes = RandomNumberGenerator.GetBytes(KeyLength);
      key = new(Convert.ToBase64String(bytes));
    }

    SessionCreated e = new()
    {
      ActorId = userId,
      OccurredOn = signedInOn,
      Key = key,
      IpAddress = ipAddress?.CleanTrim(),
      AdditionalInformation = additionalInformation?.CleanTrim(),
      CustomAttributes = customAttributes?.CleanTrim() ?? new()
    };
    new SessionCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);

    if (bytes != null)
    {
      RefreshToken = new(Id.ToGuid(), bytes);
    }
  }

  public AggregateId UserId { get; private set; }

  public bool IsPersistent => _key != null;

  public bool IsActive { get; private set; }

  public string? IpAddress { get; private set; }
  public string? AdditionalInformation { get; private set; }

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  internal RefreshToken? RefreshToken { get; private set; }

  protected virtual void Apply(SessionCreated e)
  {
    _key = e.Key;

    UserId = e.ActorId;

    IsActive = true;

    Apply((SessionSaved)e);
  }

  public void Delete(AggregateId actorId) => ApplyChange(new SessionDeleted { ActorId = actorId });
  protected virtual void Apply(SessionDeleted _) { }

  public void Refresh(byte[] bytes, string? ipAddress = null, string? additionalInformation = null,
    Dictionary<string, string>? customAttributes = null)
  {
    if (!IsActive)
    {
      throw new SessionIsNotActiveException(this);
    }
    if (_key?.IsMatch(Convert.ToBase64String(bytes)) != true)
    {
      throw new InvalidCredentialsException("The specified key did not match.");
    }

    bytes = RandomNumberGenerator.GetBytes(KeyLength);
    Pbkdf2 key = new(Convert.ToBase64String(bytes));

    SessionRefreshed e = new()
    {
      ActorId = UserId,
      Key = key,
      IpAddress = ipAddress?.CleanTrim(),
      AdditionalInformation = additionalInformation?.CleanTrim(),
      CustomAttributes = customAttributes?.CleanTrim() ?? new()
    };
    new SessionRefreshedValidator().ValidateAndThrow(e);

    ApplyChange(e);

    RefreshToken = new(Id.ToGuid(), bytes);
  }
  protected virtual void Apply(SessionRefreshed e)
  {
    _key = e.Key;

    Apply((SessionSaved)e);
  }

  public void SignOut(AggregateId actorId)
  {
    if (!IsActive)
    {
      throw new SessionIsNotActiveException(this);
    }

    ApplyChange(new SessionSignedOut { ActorId = actorId });
  }
  protected virtual void Apply(SessionSignedOut _) => IsActive = false;

  private void Apply(SessionSaved e)
  {
    IpAddress = e.IpAddress;
    AdditionalInformation = e.AdditionalInformation;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }
}
