using Logitar.EventSourcing;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Roles;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> FindAsync(string idOrUniqueSlug, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(AggregateId id, long? version = null, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(RoleAggregate session, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
}
