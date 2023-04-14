﻿using Logitar.Portal.Core.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Realms;

internal class RealmDeletedHandler : INotificationHandler<RealmDeleted>
{
  private readonly PortalContext _context;

  public RealmDeletedHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(RealmDeleted notification, CancellationToken cancellationToken)
  {
    RealmEntity realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity '{notification.AggregateId}' could not be found.");

    _context.Realms.Remove(realm);
    await _context.SaveChangesAsync(cancellationToken);
  }
}