using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Roles;

public record SearchRolesPayload : SearchPayload
{
  public new List<RoleSortOption> Sort { get; set; } = [];
}
