namespace Logitar.Portal.Domain.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> LoadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(RealmId id, bool includeDeleted, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(RealmId id, long? version, bool includeDeleted, CancellationToken cancellationToken = default);

  Task<IEnumerable<RealmAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<RealmAggregate>> LoadAsync(bool includeDeleted, CancellationToken cancellationToken = default);

  Task<IEnumerable<RealmAggregate>> LoadAsync(IEnumerable<RealmId> ids, CancellationToken cancellationToken = default);
  Task<IEnumerable<RealmAggregate>> LoadAsync(IEnumerable<RealmId> ids, bool includeDeleted, CancellationToken cancellationToken = default);

  Task<RealmAggregate?> LoadAsync(UniqueSlugUnit uniqueSlug, CancellationToken cancellationToken = default);

  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<RealmAggregate> realms, CancellationToken cancellationToken = default);
}
