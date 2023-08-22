using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> FindAsync(string idOrUniqueSlug, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(AggregateId id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
}
