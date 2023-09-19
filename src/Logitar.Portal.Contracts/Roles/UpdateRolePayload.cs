namespace Logitar.Portal.Contracts.Roles;

public record UpdateRolePayload
{
  public string? UniqueName { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public IEnumerable<CustomAttributeModification> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttributeModification>();
}
