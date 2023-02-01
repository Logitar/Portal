namespace Logitar.Portal.Contracts.Users.Payloads
{
  public class UpdateUserPayload
  {
    public string? Password { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }

    public string? Locale { get; set; }
    public string? Picture { get; set; }
  }
}
