﻿namespace Logitar.Portal.Contracts.Users;

public record CreateUserPayload
{
  public Guid? Id { get; set; }

  public string UniqueName { get; set; }
  public string? Password { get; set; }
  public bool IsDisabled { get; set; }

  public AddressPayload? Address { get; set; }
  public EmailPayload? Email { get; set; }
  public PhonePayload? Phone { get; set; }

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

  public List<CustomAttribute> CustomAttributes { get; set; }
  public List<CustomIdentifierModel> CustomIdentifiers { get; set; }
  public List<string> Roles { get; set; }

  public CreateUserPayload() : this(string.Empty)
  {
  }

  public CreateUserPayload(string uniqueName)
  {
    UniqueName = uniqueName;
    CustomAttributes = [];
    CustomIdentifiers = [];
    Roles = [];
  }
}
