using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Messages;

public interface IMessageRepository
{
  Task<Message?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<Message>> LoadAsync(CancellationToken cancellationToken = default);
  Task<IEnumerable<Message>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken = default);

  Task SaveAsync(Message message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default);
}
