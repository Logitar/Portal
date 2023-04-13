using Logitar.Portal.Contracts.Users.Contact;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Core.Claims;

public record Rfc7519Address
{
  [JsonPropertyName("formatted")]
  public string Formatted { get; set; } = string.Empty;

  [JsonPropertyName("street_address")]
  public string StreetAddress { get; set; } = string.Empty;

  [JsonPropertyName("locality")]
  public string Locality { get; set; } = string.Empty;

  [JsonPropertyName("region")]
  public string? Region { get; set; }

  [JsonPropertyName("postal_code")]
  public string? PostalCode { get; set; }

  [JsonPropertyName("country")]
  public string Country { get; set; } = string.Empty;

  public static Rfc7519Address From(Address address)
  {
    StringBuilder street_address = new();
    street_address.Append(address.Line1);
    if (address.Line2 != null)
    {
      street_address.AppendLine();
      street_address.Append(address.Line2);
    }

    return new Rfc7519Address
    {
      Formatted = address.Formatted,
      StreetAddress = street_address.ToString(),
      Locality = address.Locality,
      Region = address.Region,
      PostalCode = address.PostalCode,
      Country = address.Country
    };
  }

  public string Serialize() => JsonSerializer.Serialize(this);
}
