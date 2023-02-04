using System.Globalization;

namespace Logitar.Portal.Contracts.Users
{
  public record UpdateUserPayload
  {
    public string? Password { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }

    public CultureInfo? Locale { get; set; }
    public string? Picture { get; set; }
  }
}
