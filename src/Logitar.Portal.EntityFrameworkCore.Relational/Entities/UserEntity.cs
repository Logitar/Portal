using Logitar.Portal.Domain.Users.Events;
using System.Text.Json;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record UserEntity : AggregateEntity
{
  public UserEntity(UserCreatedEvent created) : base(created)
  {
    TenantId = created.TenantId;

    UniqueName = created.UniqueName;
  }

  private UserEntity() : base()
  {
  }

  public int UserId { get; private set; }

  public string? TenantId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }

  public bool HasPassword
  {
    get => Password != null;
    private set { }
  }
  public string? Password { get; private set; }
  public string? PasswordChangedBy { get; private set; }
  public DateTime? PasswordChangedOn { get; private set; }

  public string? DisabledBy { get; private set; }
  public DateTime? DisabledOn { get; private set; }
  public bool IsDisabled { get; private set; }

  public DateTime? AuthenticatedOn { get; private set; }

  public string? AddressStreet { get; private set; }
  public string? AddressLocality { get; private set; }
  public string? AddressRegion { get; private set; }
  public string? AddressPostalCode { get; private set; }
  public string? AddressCountry { get; private set; }
  public string? AddressFormatted { get; private set; }
  public string? AddressVerifiedBy { get; private set; }
  public DateTime? AddressVerifiedOn { get; private set; }
  public bool IsAddressVerified { get; private set; }

  public string? EmailAddress { get; private set; }
  public string? EmailAddressNormalized
  {
    get => EmailAddress?.ToUpper();
    private set { }
  }
  public string? EmailVerifiedBy { get; private set; }
  public DateTime? EmailVerifiedOn { get; private set; }
  public bool IsEmailVerified { get; private set; }

  public string? PhoneCountryCode { get; private set; }
  public string? PhoneNumber { get; private set; }
  public string? PhoneExtension { get; private set; }
  public string? PhoneE164Formatted { get; private set; }
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

  public Dictionary<string, string> CustomAttributes { get; private set; } = new();
  public string? CustomAttributesSerialized
  {
    get => CustomAttributes.Any() ? JsonSerializer.Serialize(CustomAttributes) : null;
    private set
    {
      if (value == null)
      {
        CustomAttributes.Clear();
      }
      else
      {
        CustomAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new();
      }
    }
  }

  public List<RoleEntity> Roles { get; } = new();
  public List<SessionEntity> Sessions { get; } = new();

  public void SignIn(UserSignedInEvent signedIn)
  {
    Update(signedIn);

    AuthenticatedOn = signedIn.OccurredOn;
  }

  public void Update(UserUpdatedEvent updated)
  {
    if (updated.Password != null)
    {
      Password = updated.Password.Encode();
    }

    if (updated.Email != null)
    {
      EmailAddress = updated.Email.Value?.Address;

      if (!IsEmailVerified && updated.Email.Value?.IsVerified == true)
      {
        EmailVerifiedBy = updated.ActorId.Value;
        EmailVerifiedOn = updated.OccurredOn.ToUniversalTime();
        IsEmailVerified = true;
      }
      else if (IsEmailVerified && updated.Email.Value?.IsVerified != true)
      {
        EmailVerifiedBy = null;
        EmailVerifiedOn = null;
        IsEmailVerified = false;
      }
    }

    if (updated.FirstName != null)
    {
      FirstName = updated.FirstName.Value;
    }
    if (updated.LastName != null)
    {
      LastName = updated.LastName.Value;
    }
    if (updated.FullName != null)
    {
      FullName = updated.FullName.Value;
    }
  }
}
