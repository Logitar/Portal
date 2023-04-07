using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Users.Contact;
using System.Text;

namespace Logitar.Portal.v2.Core.Users.Contact;

public record ReadOnlyAddress : ReadOnlyContact
{
  public ReadOnlyAddress(AddressInput input) : this(input.Line1, input.Locality, input.Country,
    input.Line2, input.PostalCode, input.Region, input.Verify)
  {
  }
  public ReadOnlyAddress(string line1, string locality, string country, string? line2 = null,
    string? postalCode = null, string? region = null, bool isVerified = false) : base(isVerified)
  {
    Line1 = line1.Trim();
    Line2 = line2?.CleanTrim();
    Locality = locality.Trim();
    PostalCode = postalCode?.CleanTrim();
    Country = country.Trim();
    Region = region?.CleanTrim();
    Formatted = Format();
  }

  public string Line1 { get; }
  public string? Line2 { get; }
  public string Locality { get; }
  public string? PostalCode { get; }
  public string Country { get; }
  public string? Region { get; }
  public string Formatted { get; }

  public ReadOnlyAddress AsVerified() => new(Line1, Locality, Country, Line2, PostalCode, Region, isVerified: true);

  private string Format()
  {
    StringBuilder formatted = new();

    formatted.AppendLine(Line1);

    if (Line2 != null)
    {
      formatted.AppendLine(Line2);
    }

    formatted.Append(Locality);
    if (Region != null)
    {
      formatted.Append(' ').Append(Region);
    }
    if (PostalCode != null)
    {
      formatted.Append(' ').Append(PostalCode);
    }
    formatted.AppendLine();

    formatted.Append(Country);

    return formatted.ToString();
  }
}
