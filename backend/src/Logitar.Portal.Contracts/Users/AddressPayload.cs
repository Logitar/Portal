using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record AddressPayload : IAddress
{
  public string Street { get; set; }
  public string Locality { get; set; }
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; }
  public bool IsVerified { get; set; }

  public AddressPayload() : this(string.Empty, string.Empty, null, null, string.Empty)
  {
  }

  public AddressPayload(string street, string locality, string? postalCode, string? region, string country, bool isVerified = false)
  {
    Street = street;
    Locality = locality;
    PostalCode = postalCode;
    Region = region;
    Country = country;
    IsVerified = isVerified;
  }
}
