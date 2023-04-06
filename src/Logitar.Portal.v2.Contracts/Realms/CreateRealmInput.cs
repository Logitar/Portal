namespace Logitar.Portal.v2.Contracts.Realms;

public record CreateRealmInput
{
  public string UniqueName { get; set; } = string.Empty; // TODO(fpion): renamed from Alias
  public string? DisplayName { get; set; } // TODO(fpion): renamed from Name
  public string? Description { get; set; }

  public string? DefaultLocale { get; set; }
  public string? Secret { get; set; }
  public string? Url { get; set; }

  public bool RequireConfirmedAccount { get; set; }
  public bool RequireUniqueEmail { get; set; }

  public UsernameSettings UsernameSettings { get; set; } = new(); // TODO(fpion): refactored from AllowedUsernameCharacters
  public PasswordSettings PasswordSettings { get; set; } = new();

  public IEnumerable<ClaimMapping> ClaimMappings { get; set; } = Enumerable.Empty<ClaimMapping>(); // TODO(fpion): added

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>(); // TODO(fpion): added

  // TODO(fpion): removed GoogleClientId
}
