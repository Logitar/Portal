namespace Logitar.Portal.Contracts.Roles;

public record SearchRolesPayload : SearchPayload
{
  public string? Realm { get; set; }

  public new IEnumerable<RoleSortOption> Sort { get; set; } = Enumerable.Empty<RoleSortOption>();
}
