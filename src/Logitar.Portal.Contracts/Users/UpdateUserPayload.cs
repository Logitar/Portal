namespace Logitar.Portal.Contracts.Users;

public record UpdateUserPayload
{
  public string? UniqueName { get; set; }
  public ChangePasswordPayload? Password { get; set; }
  public bool? IsDisabled { get; set; }

  public MayBe<AddressPayload>? Address { get; set; }
  public MayBe<EmailPayload>? Email { get; set; }
  public MayBe<PhonePayload>? Phone { get; set; }

  public MayBe<string>? FirstName { get; set; }
  public MayBe<string>? MiddleName { get; set; }
  public MayBe<string>? LastName { get; set; }
  public MayBe<string>? Nickname { get; set; }

  public MayBe<DateTime>? Birthdate { get; set; }
  public MayBe<string>? Gender { get; set; }
  public MayBe<string>? Locale { get; set; }
  public MayBe<string>? TimeZone { get; set; }

  public MayBe<string>? Picture { get; set; }
  public MayBe<string>? Profile { get; set; }
  public MayBe<string>? Website { get; set; }

  // TODO(fpion): Roles

  public IEnumerable<CustomAttributeModification> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttributeModification>();
}
