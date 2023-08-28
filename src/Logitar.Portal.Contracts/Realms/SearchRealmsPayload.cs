namespace Logitar.Portal.Contracts.Realms;

public record SearchRealmsPayload : SearchPayload
{
  public new IEnumerable<RealmSortOption> Sort { get; set; } = Enumerable.Empty<RealmSortOption>();
}
