namespace Logitar.Portal.Contracts.Users
{
  public record CreateUserPayload
  {
    public string? Realm { get; set; }

    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }

    public string? Locale { get; set; }
    public string? Picture { get; set; }

    public bool IsEmailConfirmed { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
  }
}
