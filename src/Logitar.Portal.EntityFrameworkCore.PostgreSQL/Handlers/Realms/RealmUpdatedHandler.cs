using Logitar.Portal.Core.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Realms;

internal class RealmUpdatedHandler : INotificationHandler<RealmUpdated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public RealmUpdatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(RealmUpdated notification, CancellationToken cancellationToken)
  {
    RealmEntity realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    realm.Update(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
