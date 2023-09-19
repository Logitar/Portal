﻿using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Contracts.Users;

public record User : Aggregate
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;

  public bool HasPassword { get; set; }
  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }

  public Actor? DisabledBy { get; set; }
  public DateTime? DisabledOn { get; set; }
  public bool IsDisabled { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

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

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  public IEnumerable<Identifier> Identifiers { get; set; } = Enumerable.Empty<Identifier>();
  public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();

  public Realm? Realm { get; set; }
}
