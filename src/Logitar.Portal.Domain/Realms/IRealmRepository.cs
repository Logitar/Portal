using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> LoadAsync(UserAggregate user, CancellationToken cancellationToken = default);
}
