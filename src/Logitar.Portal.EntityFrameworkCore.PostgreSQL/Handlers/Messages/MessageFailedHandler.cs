using Logitar.Portal.Core.Messages.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Messages;

internal class MessageFailedHandler : INotificationHandler<MessageFailed>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public MessageFailedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(MessageFailed notification, CancellationToken cancellationToken)
  {
    MessageEntity message = await _context.Messages
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The message entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    message.Fail(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
