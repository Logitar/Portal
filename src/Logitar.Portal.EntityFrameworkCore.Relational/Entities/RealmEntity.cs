using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Realms.Events;
using System.Text.Json;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record RealmEntity : AggregateEntity
{
  public RealmEntity(RealmCreatedEvent created) : base(created)
  {
    UniqueSlug = created.UniqueSlug;

    Secret = created.Secret;

    RequireUniqueEmail = created.RequireUniqueEmail;
    RequireConfirmedAccount = created.RequireConfirmedAccount;
  }

  private RealmEntity() : base()
  {
  }

  public int RealmId { get; private set; }

  public string UniqueSlug { get; private set; } = string.Empty;
  public string UniqueSlugNormalized
  {
    get => UniqueSlug.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? DefaultLocale { get; private set; }
  public string Secret { get; private set; } = string.Empty;
  public string? Url { get; private set; }

  public bool RequireUniqueEmail { get; private set; }
  public bool RequireConfirmedAccount { get; private set; }

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; private set; } = new();
  public string UniqueNameSettingsSerialized
  {
    get => JsonSerializer.Serialize(UniqueNameSettings);
    private set => UniqueNameSettings = JsonSerializer.Deserialize<ReadOnlyUniqueNameSettings>(value) ?? new();
  }
  public ReadOnlyPasswordSettings PasswordSettings { get; private set; } = new();
  public string PasswordSettingsSerialized
  {
    get => JsonSerializer.Serialize(PasswordSettings);
    private set => PasswordSettings = JsonSerializer.Deserialize<ReadOnlyPasswordSettings>(value) ?? new();
  }

  public Dictionary<string, ReadOnlyClaimMapping> ClaimMappings { get; private set; } = new();
  public string? ClaimMappingsSerialized
  {
    get => ClaimMappings.Any() ? JsonSerializer.Serialize(ClaimMappings) : null;
    private set
    {
      if (value == null)
      {
        ClaimMappings.Clear();
      }
      else
      {
        ClaimMappings = JsonSerializer.Deserialize<Dictionary<string, ReadOnlyClaimMapping>>(value) ?? new();
      }
    }
  }

  public Dictionary<string, string> CustomAttributes { get; private set; } = new();
  public string? CustomAttributesSerialized
  {
    get => CustomAttributes.Any() ? JsonSerializer.Serialize(CustomAttributes) : null;
    private set
    {
      if (value == null)
      {
        CustomAttributes.Clear();
      }
      else
      {
        CustomAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new();
      }
    }
  }

  public void Update(RealmUpdatedEvent updated)
  {
    base.Update(updated);

    if (updated.UniqueSlug != null)
    {
      UniqueSlug = updated.UniqueSlug;
    }
    if (updated.DisplayName != null)
    {
      DisplayName = updated.DisplayName.Value;
    }
    if (updated.Description != null)
    {
      Description = updated.Description.Value;
    }

    if (updated.DefaultLocale != null)
    {
      DefaultLocale = updated.DefaultLocale.Value?.Name;
    }
    if (updated.Secret != null)
    {
      Secret = updated.Secret;
    }
    if (updated.Url != null)
    {
      Url = updated.Url.Value?.ToString();
    }

    if (updated.RequireUniqueEmail.HasValue)
    {
      RequireUniqueEmail = updated.RequireUniqueEmail.Value;
    }
    if (updated.RequireConfirmedAccount.HasValue)
    {
      RequireConfirmedAccount = updated.RequireConfirmedAccount.Value;
    }

    if (updated.UniqueNameSettings != null)
    {
      UniqueNameSettings = updated.UniqueNameSettings;
    }
    if (updated.PasswordSettings != null)
    {
      PasswordSettings = updated.PasswordSettings;
    }

    foreach (KeyValuePair<string, ReadOnlyClaimMapping?> claimMapping in updated.ClaimMappings)
    {
      if (claimMapping.Value == null)
      {
        ClaimMappings.Remove(claimMapping.Key);
      }
      else
      {
        ClaimMappings[claimMapping.Key] = claimMapping.Value; ;
      }
    }

    foreach (KeyValuePair<string, string?> customAttribute in updated.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        CustomAttributes.Remove(customAttribute.Key);
      }
      else
      {
        CustomAttributes[customAttribute.Key] = customAttribute.Value;
      }
    }
  }
}
