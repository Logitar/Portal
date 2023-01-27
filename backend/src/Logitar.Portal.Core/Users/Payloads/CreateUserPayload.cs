namespace Logitar.Portal.Core.Users.Payloads
{
  public class CreateUserPayload
  {
    public string? Realm { get; set; }

    public string Username { get; set; } = null!;
    public string? Password { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }

    public string? Locale { get; set; }
    public string? Picture { get; set; }

    public bool ConfirmEmail { get; set; }
    public bool ConfirmPhoneNumber { get; set; }
  }
}
