﻿using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Realms;

internal class RealmUpdatedEventHandler : INotificationHandler<RealmUpdatedEvent>
{
  private readonly PortalContext _context;

  public RealmUpdatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(RealmUpdatedEvent @event, CancellationToken cancellationToken)
  {
    RealmEntity realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<RealmEntity>(@event.AggregateId);

    SenderEntity? sender = null;
    if (@event.PasswordRecoverySenderId?.Value.HasValue == true)
    {
      string senderId = @event.PasswordRecoverySenderId.Value.Value.ToString();
      sender = await _context.Senders
        .SingleOrDefaultAsync(x => x.AggregateId == senderId, cancellationToken)
        ?? throw new EntityNotFoundException<SenderEntity>(senderId);
    }

    realm.Update(@event, sender);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
