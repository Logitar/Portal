using Logitar.Portal.v2.Core.Realms;

namespace Logitar.Portal.v2.Core.Users;

public interface IUserRepository
{
  Task<UserAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<UserAggregate>> LoadAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
  Task<UserAggregate?> LoadAsync(RealmAggregate? realm, string username, CancellationToken cancellationToken = default);
  Task SaveAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<UserAggregate> users, CancellationToken cancellationToken = default);
}
