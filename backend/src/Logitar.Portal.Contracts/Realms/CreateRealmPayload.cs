﻿using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Realms;

public record CreateRealmPayload
{
  public string? Id { get; set; }

  public string UniqueSlug { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? DefaultLocale { get; set; }
  public string? Secret { get; set; }
  public string? Url { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; }
  public PasswordSettings PasswordSettings { get; set; }
  public bool RequireUniqueEmail { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public CreateRealmPayload() : this(string.Empty, new UniqueNameSettings(), new PasswordSettings(), requireUniqueEmail: true)
  {
  }

  public CreateRealmPayload(string uniqueSlug, UniqueNameSettings uniqueNameSettings, PasswordSettings passwordSettings, bool requireUniqueEmail)
  {
    UniqueSlug = uniqueSlug;
    UniqueNameSettings = uniqueNameSettings;
    PasswordSettings = passwordSettings;
    RequireUniqueEmail = requireUniqueEmail;
    CustomAttributes = [];
  }
}
