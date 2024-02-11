using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Models.Users;

public record UpdateProfileModel
{
  public ChangePasswordModel? Password { get; set; }

  public Modification<AddressModel>? Address { get; set; }
  public Modification<EmailModel>? Email { get; set; }
  public Modification<PhoneModel>? Phone { get; set; }

  public Modification<string>? FirstName { get; set; }
  public Modification<string>? MiddleName { get; set; }
  public Modification<string>? LastName { get; set; }
  public Modification<string>? Nickname { get; set; }

  public Modification<DateTime?>? Birthdate { get; set; }
  public Modification<string>? Gender { get; set; }
  public Modification<string>? Locale { get; set; }
  public Modification<string>? TimeZone { get; set; }

  public Modification<string>? Picture { get; set; }
  public Modification<string>? Profile { get; set; }
  public Modification<string>? Website { get; set; }

  public UpdateUserPayload ToPayload()
  {
    UpdateUserPayload payload = new()
    {
      Password = Password?.ToPayload(),
      FirstName = FirstName,
      MiddleName = MiddleName,
      LastName = LastName,
      Nickname = Nickname,
      Birthdate = Birthdate,
      Gender = Gender,
      Locale = Locale,
      TimeZone = TimeZone,
      Picture = Picture,
      Profile = Profile,
      Website = Website
    };

    if (Address != null)
    {
      payload.Address = new Modification<AddressPayload>(Address.Value?.ToPayload());
    }
    if (Email != null)
    {
      payload.Email = new Modification<EmailPayload>(Email.Value?.ToPayload());
    }
    if (Phone != null)
    {
      payload.Phone = new Modification<PhonePayload>(Phone.Value?.ToPayload());
    }

    return payload;
  }
}
