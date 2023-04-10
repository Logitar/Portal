using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Security;
using Logitar.Portal.v2.Core.Sessions.Events;
using Logitar.Portal.v2.Core.Sessions.Validators;
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
      CustomAttributes = customAttributes?.CleanTrim() ?? new()
    };
    new SessionCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);

    if (bytes != null)
    {
      RefreshToken = new(Id.ToGuid(), bytes);
    }
  }

  public bool IsPersistent => _key != null;

  public bool IsActive { get; private set; }

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  internal RefreshToken? RefreshToken { get; private set; }

  protected virtual void Apply(SessionCreated e)
  {
    _key = e.Key;

    IsActive = true;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }
}
