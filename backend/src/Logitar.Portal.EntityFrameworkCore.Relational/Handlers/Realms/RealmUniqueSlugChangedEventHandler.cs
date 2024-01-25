using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Realms;

internal class RealmUniqueSlugChangedEventHandler : INotificationHandler<RealmUniqueSlugChangedEvent>
{
  private readonly PortalContext _context;

  public RealmUniqueSlugChangedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(RealmUniqueSlugChangedEvent @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (realm != null)
    {
      realm.SetUniqueSlug(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
