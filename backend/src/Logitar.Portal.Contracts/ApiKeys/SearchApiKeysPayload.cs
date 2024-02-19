using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.ApiKeys;

public record SearchApiKeysPayload : SearchPayload
{
  public bool? HasAuthenticated { get; set; }
  public bool? IsExpired { get; set; }
  // TODO(fpion): RoleId

  public new List<ApiKeySortOption> Sort { get; set; } = [];
}
