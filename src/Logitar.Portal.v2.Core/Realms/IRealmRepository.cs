using Logitar.Portal.v2.Core.Senders;
using Logitar.Portal.v2.Core.Users;

namespace Logitar.Portal.v2.Core.Realms;

public interface IRealmRepository
{
  Task<RealmAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadAsync(string idOrUniqueName, CancellationToken cancellationToken = default);
  Task<RealmAggregate> LoadAsync(SenderAggregate sender, CancellationToken cancellationToken = default);
  Task<RealmAggregate> LoadAsync(UserAggregate user, CancellationToken cancellationToken = default);
  Task<RealmAggregate?> LoadByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task SaveAsync(RealmAggregate realm, CancellationToken cancellationToken = default);
}
