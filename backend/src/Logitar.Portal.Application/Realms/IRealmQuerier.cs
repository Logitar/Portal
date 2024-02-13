using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

public interface IRealmQuerier
{
  Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
  Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
}
