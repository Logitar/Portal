using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Realms;

internal class RealmCreatedEventHandler : INotificationHandler<RealmCreatedEvent>
{
  private readonly PortalContext _context;

  public RealmCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(RealmCreatedEvent @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (realm == null)
    {
      realm = new(@event);

      _context.Realms.Add(realm);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
