using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Core.Realms;

internal interface IRealmQuerier
{
  Task<Realm?> GetAsync(string idOrUniqueName, CancellationToken cancellationToken = default);
  Task<Realm> GetAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
}
