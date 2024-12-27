namespace Logitar.Portal.Contracts.Roles;

public record UpdateRolePayload
{
  public string? UniqueName { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public List<CustomAttributeModification> CustomAttributes { get; set; } = [];
}
