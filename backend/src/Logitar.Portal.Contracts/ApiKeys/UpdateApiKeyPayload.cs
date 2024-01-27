using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Contracts.ApiKeys;

public record UpdateApiKeyPayload
{
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public List<CustomAttributeModification> CustomAttributes { get; set; } = [];

  public List<RoleModification> Roles { get; set; } = [];
}
