using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Realms.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record RealmEntity : AggregateEntity
{
  public RealmEntity(RealmCreatedEvent created) : base(created)
  {
    UniqueSlug = created.UniqueSlug;

    Secret = created.Secret;

    RequireUniqueEmail = created.RequireUniqueEmail;
    RequireConfirmedAccount = created.RequireConfirmedAccount;

    SetUniqueNameSettings(created.UniqueNameSettings);
    SetPasswordSettings(created.PasswordSettings);
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

  public string? AllowedUniqueNameCharacters { get; private set; }

  public int RequiredPasswordLength { get; private set; }
  public int RequiredPasswordUniqueChars { get; private set; }
  public bool PasswordsRequireNonAlphanumeric { get; private set; }
  public bool PasswordsRequireLowercase { get; private set; }
  public bool PasswordsRequireUppercase { get; private set; }
  public bool PasswordsRequireDigit { get; private set; }
  public string PasswordStrategy { get; private set; } = string.Empty;

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

  public SenderEntity? PasswordRecoverySender { get; private set; }
  public int? PasswordRecoverySenderId { get; private set; }

  public void Update(RealmUpdatedEvent updated, SenderEntity? passwordRecoverySender)
  {
    Update(updated);

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
      DefaultLocale = updated.DefaultLocale.Value?.Code;
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
      SetUniqueNameSettings(updated.UniqueNameSettings);
    }
    if (updated.PasswordSettings != null)
    {
      SetPasswordSettings(updated.PasswordSettings);
    }

    foreach (KeyValuePair<string, ReadOnlyClaimMapping?> claimMapping in updated.ClaimMappings)
    {
      if (claimMapping.Value == null)
      {
        ClaimMappings.Remove(claimMapping.Key);
      }
      else
      {
        ClaimMappings[claimMapping.Key] = claimMapping.Value;
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

    if (updated.PasswordRecoverySenderId != null)
    {
      PasswordRecoverySender = passwordRecoverySender;
      PasswordRecoverySenderId = passwordRecoverySender?.SenderId;
    }
  }

  private void SetUniqueNameSettings(IUniqueNameSettings uniqueNameSettings)
  {
    AllowedUniqueNameCharacters = uniqueNameSettings.AllowedCharacters;
  }
  private void SetPasswordSettings(IPasswordSettings passwordSettings)
  {
    RequiredPasswordLength = passwordSettings.RequiredLength;
    RequiredPasswordUniqueChars = passwordSettings.RequiredUniqueChars;
    PasswordsRequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric;
    PasswordsRequireLowercase = passwordSettings.RequireLowercase;
    PasswordsRequireUppercase = passwordSettings.RequireUppercase;
    PasswordsRequireDigit = passwordSettings.RequireDigit;
    PasswordStrategy = passwordSettings.Strategy;
  }
}
