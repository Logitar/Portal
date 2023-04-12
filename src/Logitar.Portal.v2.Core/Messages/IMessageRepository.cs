namespace Logitar.Portal.v2.Core.Messages;

public interface IMessageRepository
{
  Task SaveAsync(MessageAggregate message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken = default);
}
