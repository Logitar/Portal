using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record PhoneModel : IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }
  public bool IsVerified { get; set; }

  public PhonePayload ToPayload() => new(CountryCode, Number, Extension, isVerified: false);
}
