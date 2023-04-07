using Logitar.Portal.v2.Contracts.Users.Contact;

namespace Logitar.Portal.v2.Contracts.Users;

public record CreateUserInput
{
  public string? Realm { get; set; }

  public string Username { get; set; } = string.Empty;
  public string? Password { get; set; }

  public AddressInput? Address { get; set; } // TODO(fpion): added
  public EmailInput? Email { get; set; } // TODO(fpion): from Email & ConfirmEmail
  public PhoneInput? Phone { get; set; } // TODO(fpion): from PhoneNumber & ConfirmPhoneNumber

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? Nickname { get; set; } // TODO(fpion): added

  public DateTime? Birthdate { get; set; } // TODO(fpion): added
  public string? Gender { get; set; } // TODO(fpion): added

  public string? Locale { get; set; }
  public string? TimeZone { get; set; } // TODO(fpion): added

  public string? Picture { get; set; }
  public string? Profile { get; set; } // TODO(fpion): added
  public string? Website { get; set; } // TODO(fpion): added

  public IEnumerable<CustomAttribute>? CustomAttributes { get; set; } // TODO(fpion): added
}
