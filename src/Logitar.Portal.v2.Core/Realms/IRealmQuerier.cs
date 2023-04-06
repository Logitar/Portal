using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Core.Realms;

internal interface IRealmQuerier
{
  Task<Realm?> GetAsync(string idOrUniqueName, CancellationToken cancellationToken = default);
  Task<Realm> GetAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task<PagedList<Realm>> GetAsync(string? search = null, RealmSort? sort = null, bool isDescending = false,
    int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
}
