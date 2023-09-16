using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Messages;

public record RealmSummary
{
  public AggregateId Id { get; init; }
  public string UniqueSlug { get; init; } = string.Empty;
  public string? DisplayName { get; init; }

  public static RealmSummary From(RealmAggregate realm) => new()
  {
    Id = realm.Id,
    UniqueSlug = realm.UniqueSlug,
    DisplayName = realm.DisplayName
  };
}
