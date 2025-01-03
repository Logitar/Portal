﻿using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Realms;

public record CreateRealmPayload
{
  public Guid? Id { get; set; }

  public string UniqueSlug { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? DefaultLocale { get; set; }
  public string Secret { get; set; }
  public string? Url { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; }
  public PasswordSettings PasswordSettings { get; set; }
  public bool RequireUniqueEmail { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public CreateRealmPayload() : this(string.Empty, string.Empty)
  {
  }

  public CreateRealmPayload(string uniqueSlug, string secret)
  {
    UniqueSlug = uniqueSlug;
    Secret = secret;
    UniqueNameSettings = new();
    PasswordSettings = new();
    RequireUniqueEmail = true;
    CustomAttributes = [];
  }
}
