using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Messages;

public interface IMessageRepository
{
  Task<IEnumerable<MessageAggregate>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<MessageAggregate>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);

  Task SaveAsync(MessageAggregate message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken = default);
}
