using Logitar.Portal.v2.Contracts.Actors;
using Logitar.Portal.v2.Contracts.Realms;
using Logitar.Portal.v2.Contracts.Users.Contact;

namespace Logitar.Portal.v2.Contracts.Users;

public record User : Aggregate
{
  public Guid Id { get; set; }

  public string Username { get; set; } = string.Empty;

  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }
  public bool HasPassword { get; set; }

  public Actor? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

  public DateTime? SignedInOn { get; set; }

  public Address? Address { get; set; }
  public Email? Email { get; set; }
  public Phone? Phone { get; set; }
  public bool IsConfirmed { get; set; }

  public string? FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string? LastName { get; set; }
  public string? FullName { get; set; }
  public string? Nickname { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }

  public string? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }
  public string? Profile { get; set; }
  public string? Website { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  public IEnumerable<ExternalIdentifier> ExternalIdentifiers { get; set; } = Enumerable.Empty<ExternalIdentifier>();

  public Realm Realm { get; set; } = new();
}
