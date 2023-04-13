using Logitar.Portal.Core.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Senders;

internal class SenderSetDefaultHandler : INotificationHandler<SenderSetDefault>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public SenderSetDefaultHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(SenderSetDefault notification, CancellationToken cancellationToken)
  {
    SenderEntity sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    sender.SetDefault(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
