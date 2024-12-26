using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Senders;

public interface ISenderRepository
{
  Task<Sender?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> LoadAsync(SenderId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Sender>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Sender>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);
  Task<Sender?> LoadDefaultAsync(TenantId? tenantId, CancellationToken cancellationToken = default);

  Task SaveAsync(Sender sender, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Sender> senders, CancellationToken cancellationToken = default);
}
