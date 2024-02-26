using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Realms;

public record SearchRealmsPayload : SearchPayload
{
  public new List<RealmSortOption> Sort { get; set; } = [];
}
