namespace Logitar.Portal.Contracts.Roles;

public record ReplaceRolePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public ReplaceRolePayload() : this(string.Empty)
  {
  }

  public ReplaceRolePayload(string uniqueName)
  {
    UniqueName = uniqueName;
    CustomAttributes = [];
  }
}
