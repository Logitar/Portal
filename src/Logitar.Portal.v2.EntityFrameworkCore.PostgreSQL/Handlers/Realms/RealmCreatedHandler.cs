using Logitar.Portal.v2.Core.Realms.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Realms;

internal class RealmCreatedHandler : INotificationHandler<RealmCreated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public RealmCreatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(RealmCreated notification, CancellationToken cancellationToken)
  {
    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    RealmEntity realm = new(notification, actor);

    _context.Realms.Add(realm);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
