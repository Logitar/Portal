using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record PhoneModel : IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }

  public PhoneModel() : this(null, string.Empty, null)
  {
  }

  public PhoneModel(string? countryCode, string number, string? extension)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
  }

  public PhonePayload ToPayload() => new(CountryCode, Number, Extension, isVerified: false);
}
