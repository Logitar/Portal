using Logitar.EventSourcing;
using Logitar.Portal.v2.Core.Realms;

namespace Logitar.Portal.v2.Core.Messages.Summaries;

public record RealmSummary
{
  public AggregateId Id { get; init; }
  public string UniqueName { get; init; } = string.Empty;
  public string? DisplayName { get; init; }

  public static RealmSummary From(RealmAggregate realm) => new()
  {
    Id = realm.Id,
    UniqueName = realm.UniqueName,
    DisplayName = realm.DisplayName
  };
}
