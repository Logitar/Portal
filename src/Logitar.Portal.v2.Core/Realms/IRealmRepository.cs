namespace Logitar.Portal.v2.Core.Realms;

internal interface IRealmRepository
{
  Task<RealmAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
}
