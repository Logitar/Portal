namespace Logitar.Portal.Contracts.Realms;

public record CreateRealmPayload
{
  public string UniqueSlug { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? DefaultLocale { get; set; }
  public string? Secret { get; set; }
  public string? Url { get; set; }

  public bool RequireUniqueEmail { get; set; }
  public bool RequireConfirmedAccount { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }

  public IEnumerable<ClaimMapping> ClaimMappings { get; set; } = Enumerable.Empty<ClaimMapping>();

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
