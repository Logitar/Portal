using Logitar.Portal.v2.Core.Senders.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Senders;

internal class SenderUpdatedHandler : INotificationHandler<SenderUpdated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public SenderUpdatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(SenderUpdated notification, CancellationToken cancellationToken)
  {
    SenderEntity sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    sender.Update(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
