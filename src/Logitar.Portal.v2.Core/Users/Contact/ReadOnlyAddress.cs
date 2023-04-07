using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Users.Contact;

namespace Logitar.Portal.v2.Core.Users.Contact;

public record ReadOnlyAddress : ReadOnlyContact
{
  public ReadOnlyAddress(AddressInput input) : base(input.Verify)
  {
    Line1 = input.Line1.Trim();
    Line2 = input.Line2?.CleanTrim();
    Locality = input.Locality.Trim();
    PostalCode = input.PostalCode?.CleanTrim();
    Country = input.Country.Trim();
    Region = input.Region?.CleanTrim();
  }

  public string Line1 { get; }
  public string? Line2 { get; }
  public string Locality { get; }
  public string? PostalCode { get; }
  public string Country { get; }
  public string? Region { get; }
}
