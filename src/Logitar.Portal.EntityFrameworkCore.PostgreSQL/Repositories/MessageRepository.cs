using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.Core.Messages;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Repositories;

internal class MessageRepository : EventStore, IMessageRepository
{
  public MessageRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task SaveAsync(MessageAggregate message, CancellationToken cancellationToken = default)
  {
    await base.SaveAsync(message, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken = default)
  {
    await base.SaveAsync(messages, cancellationToken);
  }
}
