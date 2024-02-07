using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Contracts.Realms;

public record UpdateRealmPayload
{
  public string? UniqueSlug { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public Modification<string>? DefaultLocale { get; set; }
  public string? Secret { get; set; }
  public Modification<string>? Url { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }
  public bool? RequireUniqueEmail { get; set; }

  public List<CustomAttributeModification> CustomAttributes { get; set; } = [];
}
