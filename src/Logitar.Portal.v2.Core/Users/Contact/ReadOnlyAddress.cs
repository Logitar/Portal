using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Users.Contact;
using System.Text;

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
    Formatted = Format();
  }

  public string Line1 { get; }
  public string? Line2 { get; }
  public string Locality { get; }
  public string? PostalCode { get; }
  public string Country { get; }
  public string? Region { get; }
  public string Formatted { get; }

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
