using Logitar.Identity.Domain.Realms;

namespace Logitar.Portal.Domain.Realms;

public interface IRealmRepository
{
  Task<IEnumerable<RealmAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(UniqueSlugUnit uniqueSlug, CancellationToken cancellationToken = default);

  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<RealmAggregate> realms, CancellationToken cancellationToken = default);
}
