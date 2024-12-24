using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record UpdateProfilePhone : IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }
  public bool IsVerified { get; set; }

  public UpdateProfilePhone() : this(null, string.Empty, null)
  {
  }

  public UpdateProfilePhone(string? countryCode, string number, string? extension)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
  }

  public PhonePayload ToPayload() => new(CountryCode, Number, Extension, isVerified: false);
}
