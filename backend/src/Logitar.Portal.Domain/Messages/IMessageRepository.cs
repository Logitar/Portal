using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Messages;

public interface IMessageRepository
{
  Task<MessageAggregate?> LoadAsync(MessageId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<MessageAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<MessageAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);

  Task SaveAsync(MessageAggregate message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken = default);
}
