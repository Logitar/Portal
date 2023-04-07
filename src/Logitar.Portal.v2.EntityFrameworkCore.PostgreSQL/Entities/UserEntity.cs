using Logitar.Portal.v2.Core.Users.Contact;
using Logitar.Portal.v2.Core.Users.Events;
using System.Text.Json;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

internal class UserEntity : AggregateEntity
{
  public UserEntity(UserCreated e, RealmEntity? realm, ActorEntity actor) : base(e, actor)
  {
    Realm = realm;
    RealmId = realm?.RealmId;

    Username = e.Username;

    FirstName = e.FirstName;
    MiddleName = e.MiddleName;
    LastName = e.LastName;
    FullName = e.FullName;
    Nickname = e.Nickname;

    Birthdate = e.Birthdate;
    Gender = e.Gender?.Value;

    Locale = e.Locale?.ToString();
    TimeZone = e.TimeZone?.Id;

    Picture = e.Picture?.ToString();
    Profile = e.Profile?.ToString();
    Website = e.Website?.ToString();

    CustomAttributes = e.CustomAttributes.Any() ? JsonSerializer.Serialize(e.CustomAttributes) : null;
  }

  private UserEntity() : base()
  {
  }

  public int UserId { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public string Username { get; private set; } = string.Empty;
  public string UsernameNormalized
  {
    get => Username.ToUpper();
    private set { }
  }

  public Guid? PasswordChangedById { get; private set; }
  public string? PasswordChangedBy { get; private set; }
  public DateTime? PasswordChangedOn { get; private set; }
  public string? Password { get; private set; }
  public bool HasPassword
  {
    get => Password != null;
    private set { }
  }

  public Guid? DisabledById { get; private set; }
  public string? DisabledBy { get; private set; }
  public DateTime? DisabledOn { get; private set; }
  public bool IsDisabled { get; private set; }

  public DateTime? SignedInOn { get; private set; }

  public string? AddressLine1 { get; private set; }
  public string? AddressLine2 { get; private set; }
  public string? AddressLocality { get; private set; }
  public string? AddressPostalCode { get; private set; }
  public string? AddressCountry { get; private set; }
  public string? AddressRegion { get; private set; }
  public string? AddressFormatted { get; private set; }
  public Guid? AddressVerifiedById { get; private set; }
  public string? AddressVerifiedBy { get; private set; }
  public DateTime? AddressVerifiedOn { get; private set; }
  public bool IsAddressVerified { get; private set; }

  public string? EmailAddress { get; private set; }
  public Guid? EmailVerifiedById { get; private set; }
  public string? EmailVerifiedBy { get; private set; }
  public DateTime? EmailVerifiedOn { get; private set; }
  public bool IsEmailVerified { get; private set; }

  public string? PhoneCountryCode { get; set; }
  public string? PhoneNumber { get; private set; }
  public string? PhoneExtension { get; set; }
  public string? PhoneE164Formatted { get; private set; }
  public Guid? PhoneVerifiedById { get; private set; }
  public string? PhoneVerifiedBy { get; private set; }
  public DateTime? PhoneVerifiedOn { get; private set; }
  public bool IsPhoneVerified { get; private set; }

  public bool IsConfirmed
  {
    get => IsAddressVerified || IsEmailVerified || IsPhoneVerified;
    private set { }
  }

  public string? FirstName { get; private set; }
  public string? MiddleName { get; private set; }
  public string? LastName { get; private set; }
  public string? FullName { get; private set; }
  public string? Nickname { get; private set; }

  public DateTime? Birthdate { get; private set; }
  public string? Gender { get; private set; }

  public string? Locale { get; private set; }
  public string? TimeZone { get; private set; }

  public string? Picture { get; private set; }
  public string? Profile { get; private set; }
  public string? Website { get; private set; }

  public string? CustomAttributes { get; private set; }

  public void ChangePassword(PasswordChanged e, ActorEntity actor)
  {
    SetVersion(e);

    PasswordChangedById = e.ActorId.ToGuid();
    PasswordChangedBy = actor.Serialize();
    PasswordChangedOn = e.OccurredOn;
    Password = e.Password.ToString();
  }

  public void SetAddress(AddressChanged e, ActorEntity actor)
  {
    Update(e, actor);

    AddressLine1 = e.Address?.Line1;
    AddressLine2 = e.Address?.Line2;
    AddressLocality = e.Address?.Locality;
    AddressPostalCode = e.Address?.PostalCode;
    AddressCountry = e.Address?.Country;
    AddressRegion = e.Address?.Region;
    AddressFormatted = e.Address?.Formatted;

    switch (e.VerificationAction)
    {
      case VerificationAction.Unverify:
        AddressVerifiedById = null;
        AddressVerifiedBy = null;
        AddressVerifiedOn = null;
        IsAddressVerified = false;
        break;
      case VerificationAction.Verify:
        AddressVerifiedById = e.ActorId.ToGuid();
        AddressVerifiedBy = actor.Serialize();
        AddressVerifiedOn = e.OccurredOn;
        IsAddressVerified = true;
        break;
    }
  }

  public void SetEmail(EmailChanged e, ActorEntity actor)
  {
    Update(e, actor);

    EmailAddress = e.Email?.Address;

    switch (e.VerificationAction)
    {
      case VerificationAction.Unverify:
        EmailVerifiedById = null;
        EmailVerifiedBy = null;
        EmailVerifiedOn = null;
        IsEmailVerified = false;
        break;
      case VerificationAction.Verify:
        EmailVerifiedById = e.ActorId.ToGuid();
        EmailVerifiedBy = actor.Serialize();
        EmailVerifiedOn = e.OccurredOn;
        IsEmailVerified = true;
        break;
    }
  }

  public void SetPhone(PhoneChanged e, ActorEntity actor)
  {
    Update(e, actor);

    PhoneCountryCode = e.Phone?.CountryCode;
    PhoneNumber = e.Phone?.Number;
    PhoneExtension = e.Phone?.Extension;
    PhoneE164Formatted = e.Phone?.ToE164String();

    switch (e.VerificationAction)
    {
      case VerificationAction.Unverify:
        PhoneVerifiedById = null;
        PhoneVerifiedBy = null;
        PhoneVerifiedOn = null;
        IsPhoneVerified = false;
        break;
      case VerificationAction.Verify:
        PhoneVerifiedById = e.ActorId.ToGuid();
        PhoneVerifiedBy = actor.Serialize();
        PhoneVerifiedOn = e.OccurredOn;
        IsPhoneVerified = true;
        break;
    }
  }

  public override void SetActor(Guid id, ActorEntity actor)
  {
    base.SetActor(id, actor);

    if (PasswordChangedById == id)
    {
      PasswordChangedBy = actor.Serialize();
    }

    if (DisabledById == id)
    {
      DisabledBy = actor.Serialize();
    }

    if (AddressVerifiedById == id)
    {
      AddressVerifiedBy = actor.Serialize();
    }

    if (EmailVerifiedById == id)
    {
      EmailVerifiedBy = actor.Serialize();
    }

    if (PhoneVerifiedById == id)
    {
      PhoneVerifiedBy = actor.Serialize();
    }
  }
}
