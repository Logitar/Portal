﻿using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Contracts.Users;

public class User : Aggregate
{
  public string UniqueName { get; set; }

  public bool HasPassword { get; set; }
  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public Actor? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

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
  public Locale? Locale { get; set; }
  public string? TimeZone { get; set; }

  public string? Picture { get; set; }
  public string? Profile { get; set; }
  public string? Website { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }
  public List<CustomIdentifier> CustomIdentifiers { get; set; }
  public List<Role> Roles { get; set; }

  public Realm? Realm { get; set; }

  public User() : this(string.Empty)
  {
  }

  public User(string uniqueName)
  {
    UniqueName = uniqueName;
    CustomAttributes = [];
    CustomIdentifiers = [];
    Roles = [];
  }
}
