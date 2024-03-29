﻿using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Realms;

public class Realm : Aggregate
{
  public string UniqueSlug { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public Locale? DefaultLocale { get; set; }
  public string Secret { get; set; }
  public string? Url { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; }
  public PasswordSettings PasswordSettings { get; set; }
  public bool RequireUniqueEmail { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public Realm() : this(string.Empty, string.Empty)
  {
  }

  public Realm(string uniqueSlug, string secret)
  {
    UniqueSlug = uniqueSlug;
    Secret = secret;
    UniqueNameSettings = new();
    PasswordSettings = new();
    RequireUniqueEmail = true;
    CustomAttributes = [];
  }
}
