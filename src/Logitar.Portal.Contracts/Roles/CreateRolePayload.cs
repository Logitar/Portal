namespace Logitar.Portal.Contracts.Roles;

public record CreateRolePayload
{
  public string? Realm { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();
}
