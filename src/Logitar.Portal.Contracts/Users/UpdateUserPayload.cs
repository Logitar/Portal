using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Contracts.Users;

public record UpdateUserPayload
{
  public string? UniqueName { get; set; }
  public ChangePasswordPayload? Password { get; set; }
  public bool? IsDisabled { get; set; }

  public Modification<AddressPayload>? Address { get; set; }
  public Modification<EmailPayload>? Email { get; set; }
  public Modification<PhonePayload>? Phone { get; set; }

  public Modification<string>? FirstName { get; set; }
  public Modification<string>? MiddleName { get; set; }
  public Modification<string>? LastName { get; set; }
  public Modification<string>? Nickname { get; set; }

  public Modification<DateTime>? Birthdate { get; set; }
  public Modification<string>? Gender { get; set; }
  public Modification<string>? Locale { get; set; }
  public Modification<string>? TimeZone { get; set; }

  public Modification<string>? Picture { get; set; }
  public Modification<string>? Profile { get; set; }
  public Modification<string>? Website { get; set; }

  public IEnumerable<CustomAttributeModification> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttributeModification>();

  public IEnumerable<RoleModification> Roles { get; set; } = Enumerable.Empty<RoleModification>();
}
