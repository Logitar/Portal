using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

public interface IRealmQuerier
{
  Task<Realm?> FindAsync(string idOrUniqueSlug, CancellationToken cancellationToken = default);
  Task<Realm> ReadAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(AggregateId id, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
}
