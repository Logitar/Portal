﻿using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Senders;

internal class SenderUpdatedEventHandler : INotificationHandler<SenderUpdatedEvent>
{
  private readonly PortalContext _context;

  public SenderUpdatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SenderUpdatedEvent @event, CancellationToken cancellationToken)
  {
    SenderEntity role = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<SenderEntity>(@event.AggregateId);

    role.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
