using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> FindAsync(string idOrUniqueSlug, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(UserAggregate user, CancellationToken cancellationToken = default);
}
