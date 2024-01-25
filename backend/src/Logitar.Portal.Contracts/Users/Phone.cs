using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record Phone : Contact, IPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }
  public string? Extension { get; set; }
  public string E164Formatted { get; set; }

  public Phone() : this(string.Empty, string.Empty)
  {
  }

  public Phone(string number, string e164Formatted)
  {
    Number = number;
    E164Formatted = e164Formatted;
  }
}
