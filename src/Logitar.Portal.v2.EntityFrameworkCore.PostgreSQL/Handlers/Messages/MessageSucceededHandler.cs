using Logitar.Portal.v2.Core.Messages.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Messages;

internal class MessageSucceededHandler : INotificationHandler<MessageSucceeded>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public MessageSucceededHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(MessageSucceeded notification, CancellationToken cancellationToken)
  {
    MessageEntity message = await _context.Messages
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The message entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    message.Succeed(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
