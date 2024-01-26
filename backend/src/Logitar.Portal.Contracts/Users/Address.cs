using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record Address : Contact, IAddress
{
  public string Street { get; set; }
  public string Locality { get; set; }
  public string? PostalCode { get; set; }
  public string? Region { get; set; }
  public string Country { get; set; }
  public string Formatted { get; set; }

  public Address() : this(string.Empty, string.Empty, string.Empty, string.Empty)
  {
  }

  public Address(string street, string locality, string country, string formatted)
  {
    Street = street;
    Locality = locality;
    Country = country;
    Formatted = formatted;
  }
}
