namespace Logitar.Portal.Contracts.Users;

public record AddressPayload
{
  public string Street { get; set; } = string.Empty;
  public string Locality { get; set; } = string.Empty;
  public string? Region { get; set; }
  public string? PostalCode { get; set; }
  public string Country { get; set; } = string.Empty;
  public bool IsVerified { get; set; }
}
