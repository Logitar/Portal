namespace Logitar.Portal.Contracts.Users;

public record Phone : Contact
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }
  public string E164Formatted { get; set; }

  public Phone() : this(null, string.Empty, null, string.Empty)
  {
  }

  public Phone(string? countryCode, string number, string? extension, string e164Formatted)
  {
    CountryCode = countryCode;
    Number = number;
    Extension = extension;
    E164Formatted = e164Formatted;
  }
}
