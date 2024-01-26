using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record PhonePayload : IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }
  public bool IsVerified { get; set; }

  public PhonePayload() : this(null, string.Empty, null)
  {
  }

  public PhonePayload(string? countryCode, string number, string? extension, bool isVerified = false)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
    IsVerified = isVerified;
  }
}
