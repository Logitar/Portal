using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Senders;

public interface ISenderRepository
{
  Task<SenderAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SenderAggregate?> LoadAsync(SenderId id, long? version, CancellationToken cancellationToken = default);
  Task<IEnumerable<SenderAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<SenderAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<SenderAggregate?> LoadDefaultAsync(TenantId? tenantId, CancellationToken cancellationToken = default);

  Task SaveAsync(SenderAggregate sender, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<SenderAggregate> senders, CancellationToken cancellationToken = default);
}
