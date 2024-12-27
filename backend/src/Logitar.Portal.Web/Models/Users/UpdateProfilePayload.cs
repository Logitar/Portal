using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record UpdateProfilePayload
{
  public ChangePasswordModel? Password { get; set; }

  public ChangeModel<UpdateProfileAddress>? Address { get; set; }
  public ChangeModel<EmailModel>? Email { get; set; }
  public ChangeModel<PhoneModel>? Phone { get; set; }

  public ChangeModel<string>? FirstName { get; set; }
  public ChangeModel<string>? MiddleName { get; set; }
  public ChangeModel<string>? LastName { get; set; }
  public ChangeModel<string>? Nickname { get; set; }

  public ChangeModel<DateTime?>? Birthdate { get; set; }
  public ChangeModel<string>? Gender { get; set; }
  public ChangeModel<string>? Locale { get; set; }
  public ChangeModel<string>? TimeZone { get; set; }

  public ChangeModel<string>? Picture { get; set; }
  public ChangeModel<string>? Profile { get; set; }
  public ChangeModel<string>? Website { get; set; }

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
      payload.Address = new ChangeModel<AddressPayload>(Address.Value?.ToPayload());
    }
    if (Email != null)
    {
      payload.Email = new ChangeModel<EmailPayload>(Email.Value?.ToPayload());
    }
    if (Phone != null)
    {
      payload.Phone = new ChangeModel<PhonePayload>(Phone.Value?.ToPayload());
    }

    return payload;
  }
}
