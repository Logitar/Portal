using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.ApiKeys;

public record SearchApiKeysPayload : SearchPayload
{
  public bool? HasAuthenticated { get; set; }
  public ApiKeyStatus? Status { get; set; }

  public new List<ApiKeySortOption> Sort { get; set; } = [];
}
