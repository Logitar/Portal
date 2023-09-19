using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Contracts.ApiKeys;

public record UpdateApiKeyPayload
{
  public string? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public IEnumerable<CustomAttributeModification> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttributeModification>();

  public IEnumerable<RoleModification> Roles { get; set; } = Enumerable.Empty<RoleModification>();
}
