using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Security;
using Logitar.Portal.v2.Core.Sessions;
using Logitar.Portal.v2.Core.Users.Contact;
using Logitar.Portal.v2.Core.Users.Events;
using Logitar.Portal.v2.Core.Users.Validators;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Users;

public class UserAggregate : AggregateRoot
{
  private readonly Dictionary<string, string> _customAttributes = new();
  private readonly Dictionary<string, string> _externalIdentifiers = new();
  private Pbkdf2? _password = null;

  public UserAggregate(AggregateId id) : base(id)
  {
  }

  public UserAggregate(AggregateId actorId, RealmAggregate realm, string username,
    string? firstName = null, string? middleName = null, string? lastName = null, string? nickname = null,
    DateTime? birthdate = null, Gender? gender = null, CultureInfo? locale = null, TimeZoneEntry? timeZone = null,
    Uri? picture = null, Uri? profile = null, Uri? website = null,
    Dictionary<string, string>? customAttributes = null) : base()
  {
    UserCreated e = new()
    {
      ActorId = actorId,
      RealmId = realm.Id,
      Username = username.Trim(),
      FirstName = firstName?.CleanTrim(),
      MiddleName = middleName?.CleanTrim(),
      LastName = lastName?.CleanTrim(),
      FullName = GetFullName(firstName, middleName, lastName),
      Nickname = nickname?.CleanTrim(),
      Birthdate = birthdate,
      Gender = gender,
      Locale = locale,
      TimeZone = timeZone,
      Picture = picture,
      Profile = profile,
      Website = website,
      CustomAttributes = customAttributes?.CleanTrim() ?? new()
    };
    new UserCreatedValidator(realm.UsernameSettings).ValidateAndThrow(e);

    ApplyChange(e);
  }

  public AggregateId RealmId { get; private set; }

  public string Username { get; private set; } = string.Empty;
  public bool HasPassword => _password != null;

  public bool IsDisabled { get; private set; }

  public DateTime? SignedInOn { get; private set; }

  public ReadOnlyAddress? Address { get; private set; }
  public ReadOnlyEmail? Email { get; private set; }
  public ReadOnlyPhone? Phone { get; private set; }
  public bool IsConfirmed => Address?.IsVerified == true || Email?.IsVerified == true || Phone?.IsVerified == true;

  public string? FirstName { get; private set; }
  public string? MiddleName { get; private set; }
  public string? LastName { get; private set; }
  public string? FullName { get; private set; }
  public string? Nickname { get; private set; }

  public DateTime? Birthdate { get; private set; }
  public Gender? Gender { get; private set; }

  public CultureInfo? Locale { get; private set; }
  public TimeZoneEntry? TimeZone { get; private set; }

  public Uri? Picture { get; private set; }
  public Uri? Profile { get; private set; }
  public Uri? Website { get; private set; }

  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();
  public IReadOnlyDictionary<string, string> ExternalIdentifiers => _externalIdentifiers.AsReadOnly();

  protected virtual void Apply(UserCreated e)
  {
    RealmId = e.RealmId;

    Username = e.Username;

    Apply((UserSaved)e);
  }

  public void ChangePassword(AggregateId actorId, RealmAggregate realm, string password, string? current = null)
  {
    if (realm.Id != RealmId)
    {
      throw new ArgumentException("The realm must be the realm in which the user belongs.", nameof(realm));
    }
    else if (current != null && _password?.IsMatch(current) != true)
    {
      throw new InvalidCredentialsException("The specified password did not match.");
    }

    new PasswordValidator(realm.PasswordSettings).ValidateAndThrow(password);

    ApplyChange(new PasswordChanged()
    {
      ActorId = actorId,
      Password = new Pbkdf2(password)
    });
  }
  protected virtual void Apply(PasswordChanged e) => _password = e.Password;

  public void Delete(AggregateId actorId) => ApplyChange(new UserDeleted { ActorId = actorId });
  protected virtual void Apply(UserDeleted _) { }

  public void Disable(AggregateId actorId)
  {
    if (!IsDisabled)
    {
      ApplyChange(new DisabledChanged
      {
        ActorId = actorId,
        IsDisabled = true
      });
    }
  }
  public void Enable(AggregateId actorId)
  {
    if (IsDisabled)
    {
      ApplyChange(new DisabledChanged
      {
        ActorId = actorId,
        IsDisabled = false
      });
    }
  }
  protected virtual void Apply(DisabledChanged e) => IsDisabled = e.IsDisabled;

  public void SetAddress(AggregateId actorId, ReadOnlyAddress? address)
  {
    bool isModified = address?.Line1 != Address?.Line1 || address?.Line2 != Address?.Line2
      || address?.Locality != Address?.Locality || address?.PostalCode != Address?.PostalCode
      || address?.Country != Address?.Country || address?.Region != Address?.Region;

    AddressChanged e = new()
    {
      ActorId = actorId,
      Address = address,
      VerificationAction = address?.IsVerified == true ? VerificationAction.Verify
        : (isModified ? VerificationAction.Unverify : null)
    };
    new AddressChangedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(AddressChanged e) => Address = e.Address;

  public void SetEmail(AggregateId actorId, ReadOnlyEmail? email)
  {
    bool isModified = email?.Address != Email?.Address;

    EmailChanged e = new()
    {
      ActorId = actorId,
      Email = email,
      VerificationAction = email?.IsVerified == true ? VerificationAction.Verify
        : (isModified ? VerificationAction.Unverify : null)
    };
    new EmailChangedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(EmailChanged e) => Email = e.Email;

  public void SetExternalIdentifier(AggregateId actorId, string key, string? value)
  {
    key = key.Trim();
    value = value?.CleanTrim();

    if (value != null || _externalIdentifiers.ContainsKey(key))
    {
      ExternalIdentifierSet e = new()
      {
        ActorId = actorId,
        Key = key,
        Value = value
      };
      new ExternalIdentifierSetValidator().ValidateAndThrow(e);

      ApplyChange(e);
    }
  }
  protected virtual void Apply(ExternalIdentifierSet e)
  {
    if (e.Value == null)
    {
      _externalIdentifiers.Remove(e.Key);
    }
    else
    {
      _externalIdentifiers[e.Key] = e.Value;
    }
  }

  public void SetPhone(AggregateId actorId, ReadOnlyPhone? phone)
  {
    bool isModified = phone?.CountryCode != Phone?.CountryCode
      || phone?.Number != Phone?.Number || phone?.Extension != Phone?.Extension;

    PhoneChanged e = new()
    {
      ActorId = actorId,
      Phone = phone,
      VerificationAction = phone?.IsVerified == true ? VerificationAction.Verify
        : (isModified ? VerificationAction.Unverify : null)
    };
    new PhoneChangedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(PhoneChanged e) => Phone = e.Phone;

  public SessionAggregate SignIn(RealmAggregate realm, string password, bool isPersistent = false,
    string? ipAddress = null, string? additionalInformation = null,
    Dictionary<string, string>? customAttributes = null)
  {
    if (realm.Id != RealmId)
    {
      throw new ArgumentException("The realm must be the realm in which the user belongs.", nameof(realm));
    }
    else if (_password?.IsMatch(password) != true)
    {
      throw new InvalidCredentialsException("The specified password did not match.");
    }
    else if (IsDisabled)
    {
      throw new AccountIsDisabledException(this);
    }
    else if (realm.RequireConfirmedAccount && !IsConfirmed)
    {
      throw new AccountIsNotConfirmedException(this);
    }

    UserSignedIn e = new();
    ApplyChange(e);

    return new SessionAggregate(Id, e.OccurredOn, isPersistent, ipAddress, additionalInformation, customAttributes);
  }
  protected virtual void Apply(UserSignedIn e) => SignedInOn = e.OccurredOn;

  public void Update(AggregateId actorId, string? firstName, string? middleName, string? lastName,
    string? nickname, DateTime? birthdate, Gender? gender, CultureInfo? locale, TimeZoneEntry? timeZone,
    Uri? picture, Uri? profile, Uri? website, Dictionary<string, string>? customAttributes)
  {
    UserUpdated e = new()
    {
      ActorId = actorId,
      FirstName = firstName?.CleanTrim(),
      MiddleName = middleName?.CleanTrim(),
      LastName = lastName?.CleanTrim(),
      FullName = GetFullName(firstName, middleName, lastName),
      Nickname = nickname?.CleanTrim(),
      Birthdate = birthdate,
      Gender = gender,
      Locale = locale,
      TimeZone = timeZone,
      Picture = picture,
      Profile = profile,
      Website = website,
      CustomAttributes = customAttributes?.CleanTrim() ?? new()
    };
    new UserUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(UserUpdated e) => Apply((UserSaved)e);

  private void Apply(UserSaved e)
  {
    FirstName = e.FirstName;
    MiddleName = e.MiddleName;
    LastName = e.LastName;
    FullName = e.FullName;
    Nickname = e.Nickname;

    Birthdate = e.Birthdate;
    Gender = e.Gender;

    Locale = e.Locale;
    TimeZone = e.TimeZone;

    Picture = e.Picture;
    Profile = e.Profile;
    Website = e.Website;

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }

  public override string ToString() => $"{FullName ?? Username} | {base.ToString()}";

  private static string? GetFullName(params string?[] names) => string.Join(' ', names
    .SelectMany(name => name?.Split() ?? Array.Empty<string>())
    .Where(name => !string.IsNullOrEmpty(name)));
}
