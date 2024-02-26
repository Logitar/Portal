using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record AddressPayload : ContactPayload, IAddress
{
  public string Street { get; set; }
  public string Locality { get; set; }
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; }

  public AddressPayload() : this(string.Empty, string.Empty, null, null, string.Empty, isVerified: false)
  {
  }

  public AddressPayload(string street, string locality, string? postalCode, string? region, string country, bool isVerified) : base(isVerified)
  {
    Street = street;
    Locality = locality;
    PostalCode = postalCode;
    Region = region;
    Country = country;
  }
}
