using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class MessageRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IMessageRepository
{
  public MessageRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer)
    : base(eventBus, eventContext, eventSerializer)
  {
  }

  public async Task SaveAsync(MessageAggregate message, CancellationToken cancellationToken)
    => await base.SaveAsync(message, cancellationToken);
  public async Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken)
    => await base.SaveAsync(messages, cancellationToken);
}
