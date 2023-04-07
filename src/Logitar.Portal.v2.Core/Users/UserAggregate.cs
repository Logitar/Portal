using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Security;
using Logitar.Portal.v2.Core.Users.Contact;
using Logitar.Portal.v2.Core.Users.Events;
using Logitar.Portal.v2.Core.Users.Validators;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Users;

public class UserAggregate : AggregateRoot
{
  private readonly Dictionary<string, string> _customAttributes = new();
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

  public AggregateId? RealmId { get; private set; }

  public string Username { get; private set; } = string.Empty;
  public bool HasPassword => _password != null;

  public bool IsDisabled { get; private set; }

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

  protected virtual void Apply(UserCreated e)
  {
    RealmId = e.RealmId;

    Username = e.Username;

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

  public void ChangePassword(AggregateId actorId, RealmAggregate realm, string password, string? current = null)
  {
    if (current != null && _password?.IsMatch(current) != true)
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

  public void SetAddress(AggregateId actorId, ReadOnlyAddress? address)
  {
    bool isModified = address?.Line1 != Address?.Line1 || address?.Line2 != Address?.Line2
      || address?.Locality != Address?.Locality || address?.PostalCode != Address?.PostalCode
      || address?.Country != Address?.Country || address?.Region != Address?.Region;

    ApplyChange(new AddressChanged
    {
      ActorId = actorId,
      Address = address,
      VerificationAction = address?.IsVerified == true ? VerificationAction.Verify
        : (isModified ? VerificationAction.Unverify : null)
    });
  }
  protected virtual void Apply(AddressChanged e) => Address = e.Address;

  public void SetEmail(AggregateId actorId, ReadOnlyEmail? email)
  {
    bool isModified = email?.Address != Email?.Address;

    ApplyChange(new EmailChanged
    {
      ActorId = actorId,
      Email = email,
      VerificationAction = email?.IsVerified == true ? VerificationAction.Verify
        : (isModified ? VerificationAction.Unverify : null)
    });
  }
  protected virtual void Apply(EmailChanged e) => Email = e.Email;

  public void SetPhone(AggregateId actorId, ReadOnlyPhone? phone)
  {
    bool isModified = phone?.CountryCode != Phone?.CountryCode
      || phone?.Number != Phone?.Number || phone?.Extension != Phone?.Extension;

    ApplyChange(new PhoneChanged
    {
      ActorId = actorId,
      Phone = phone,
      VerificationAction = phone?.IsVerified == true ? VerificationAction.Verify
        : (isModified ? VerificationAction.Unverify : null)
    });
  }
  protected virtual void Apply(PhoneChanged e) => Phone = e.Phone;

  public override string ToString() => $"{FullName ?? Username} | {base.ToString()}";

  private static string? GetFullName(params string?[] names) => string.Join(' ', names
    .SelectMany(name => name?.Split() ?? Array.Empty<string>())
    .Where(name => !string.IsNullOrEmpty(name)));
}
