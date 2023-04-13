using Logitar.Portal.Core.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Senders;

internal class SenderCreatedHandler : INotificationHandler<SenderCreated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public SenderCreatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(SenderCreated notification, CancellationToken cancellationToken)
  {
    RealmEntity realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity '{notification.RealmId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    SenderEntity sender = new(notification, realm, actor);

    _context.Senders.Add(sender);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
