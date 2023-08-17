namespace Logitar.Portal.Contracts.Realms;

public record UpdateRealmPayload
{
  public string? UniqueSlug { get; set; }
  public MayBe<string>? DisplayName { get; set; }
  public MayBe<string>? Description { get; set; }

  public MayBe<string>? DefaultLocale { get; set; }
  public string? Secret { get; set; }
  public MayBe<string>? Url { get; set; }

  public bool? RequireUniqueEmail { get; set; }
  public bool? RequireConfirmedAccount { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }

  public IEnumerable<ClaimMappingModification> ClaimMappings { get; set; } = Enumerable.Empty<ClaimMappingModification>();

  public IEnumerable<CustomAttributeModification> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttributeModification>();
}
