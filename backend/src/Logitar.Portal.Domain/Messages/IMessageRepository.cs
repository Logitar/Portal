using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Messages;

public interface IMessageRepository
{
  Task<Message?> LoadAsync(MessageId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Message>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Message>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);

  Task SaveAsync(Message message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default);
}
