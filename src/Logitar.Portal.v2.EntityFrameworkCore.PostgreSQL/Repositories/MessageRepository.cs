﻿using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Portal.v2.Core.Messages;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Repositories;

internal class MessageRepository : EventStore, IMessageRepository
{
  public MessageRepository(EventContext context, IEventBus eventBus) : base(context, eventBus)
  {
  }

  public async Task SaveAsync(IEnumerable<MessageAggregate> messages, CancellationToken cancellationToken = default)
  {
    await base.SaveAsync(messages, cancellationToken);
  }
}
