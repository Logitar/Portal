using Logitar.Portal.Domain.Realms.Events;
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

  public async Task Handle(RealmUpdatedEvent updated, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == updated.AggregateId.Value, cancellationToken);
    if (realm != null)
    {
      realm.Update(updated);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
