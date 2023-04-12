using Logitar.Portal.v2.Core.Messages.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Messages;

internal class MessageCreatedHandler : INotificationHandler<MessageCreated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public MessageCreatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(MessageCreated notification, CancellationToken cancellationToken)
  {
    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    MessageEntity message = new(notification, actor);

    _context.Messages.Add(message);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
