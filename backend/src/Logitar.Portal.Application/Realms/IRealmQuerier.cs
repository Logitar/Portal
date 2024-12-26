using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

public interface IRealmQuerier
{
  Task<RealmModel> ReadAsync(Realm realm, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmModel?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
  Task<SearchResults<RealmModel>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
}
