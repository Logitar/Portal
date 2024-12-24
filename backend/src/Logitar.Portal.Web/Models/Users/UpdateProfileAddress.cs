using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record UpdateProfileAddress : IAddress
{
  public string Street { get; set; }
  public string Locality { get; set; }
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; }
  public bool IsVerified { get; set; }

  public UpdateProfileAddress() : this(string.Empty, string.Empty, null, null, string.Empty)
  {
  }

  public UpdateProfileAddress(string street, string locality, string? postalCode, string? region, string country)
  {
    Street = street;
    Locality = locality;
    PostalCode = postalCode;
    Region = region;
    Country = country;
  }

  public AddressPayload ToPayload() => new(Street, Locality, PostalCode, Region, Country, isVerified: false);
}
