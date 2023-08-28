using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record RealmEntity : AggregateEntity
{
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
}
