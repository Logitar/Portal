using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Messages;

public interface IMessageRepository
{
  Task<IEnumerable<MessageAggregate>> LoadAsync(RealmAggregate? realm, CancellationToken cancellationToken = default);

  Task SaveAsync(MessageAggregate message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken = default);
}
