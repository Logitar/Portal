﻿using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Realms;

internal class RealmDeletedEventHandler : INotificationHandler<RealmDeletedEvent>
{
  private readonly PortalContext _context;

  public RealmDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(RealmDeletedEvent deleted, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == deleted.AggregateId.Value, cancellationToken);
    if (realm != null)
    {
      _context.Realms.Remove(realm);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
