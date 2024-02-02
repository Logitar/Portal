using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record PhonePayload : ContactPayload, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }

  public PhonePayload() : this(null, string.Empty, null, isVerified: false)
  {
  }

  public PhonePayload(string? countryCode, string number, string? extension, bool isVerified) : base(isVerified)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
  }
}
