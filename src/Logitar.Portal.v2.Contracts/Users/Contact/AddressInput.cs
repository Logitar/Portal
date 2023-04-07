namespace Logitar.Portal.v2.Contracts.Users.Contact;

public record AddressInput : ContactInput
{
  public string Line1 { get; set; } = string.Empty;
  public string? Line2 { get; set; }
  public string Locality { get; set; } = string.Empty;
  public string? PostalCode { get; set; }
  public string Country { get; set; } = string.Empty;
  public string? Region { get; set; }
}
