using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Core.Realms;

public interface IRealmQuerier
{
  Task<Realm> GetAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task<Realm?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Realm?> GetAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task<PagedList<Realm>> GetAsync(string? search = null, RealmSort? sort = null, bool isDescending = false,
    int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
}
