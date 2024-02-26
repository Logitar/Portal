namespace Logitar.Portal.Contracts.Roles;

public record CreateRolePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public CreateRolePayload() : this(string.Empty)
  {
  }

  public CreateRolePayload(string uniqueName)
  {
    UniqueName = uniqueName;
    CustomAttributes = [];
  }
}
