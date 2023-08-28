namespace Logitar.Portal.Contracts.Users;

public record PhonePayload
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }
  public bool IsVerified { get; set; }
}
