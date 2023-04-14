using Logitar.Portal.Contracts.Users.Contact;

namespace Logitar.Portal.Contracts.Users;

public record CreateUserInput
{
  public string? Realm { get; set; }

  public string Username { get; set; } = string.Empty;
  public string? Password { get; set; }

  public AddressInput? Address { get; set; }
  public EmailInput? Email { get; set; }
  public PhoneInput? Phone { get; set; }

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? Nickname { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }

  public string? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }
  public string? Profile { get; set; }
  public string? Website { get; set; }

  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; }
}
