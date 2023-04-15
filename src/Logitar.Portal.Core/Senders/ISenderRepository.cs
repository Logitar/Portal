using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Core.Senders;

public interface ISenderRepository
{
  Task<SenderAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<SenderAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken = default);
  Task<SenderAggregate?> LoadDefaultAsync(RealmAggregate? realm, CancellationToken cancellationToken = default);
  Task SaveAsync(SenderAggregate sender, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<SenderAggregate> senders, CancellationToken cancellationToken = default);
}
