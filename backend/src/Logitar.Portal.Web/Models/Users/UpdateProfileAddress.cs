using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record UpdateProfileAddress : IAddress
{
  public string Street { get; set; } = string.Empty;
  public string Locality { get; set; } = string.Empty;
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; } = string.Empty;
  public bool IsVerified { get; set; }

  public AddressPayload ToPayload() => new(Street, Locality, PostalCode, Region, Country, isVerified: false);
}
