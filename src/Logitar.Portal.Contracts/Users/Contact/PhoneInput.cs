namespace Logitar.Portal.Contracts.Users.Contact;

public record PhoneInput : ContactInput
{
  public string? CountryCode { get; set; }
  public string Number { get; set; } = string.Empty;
  public string? Extension { get; set; }
}
