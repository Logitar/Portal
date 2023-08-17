using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

public interface IRealmQuerier
{
  Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(string id, CancellationToken cancellationToken = default);
  Task<Realm?> ReadByUniqueSlugAsync(string uniqueSlug, CancellationToken cancellationToken = default);
  Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
}
