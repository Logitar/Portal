using Logitar.Portal.Core.Sessions.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Sessions;

internal class SessionSignedOutHandler : INotificationHandler<SessionSignedOut>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public SessionSignedOutHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(SessionSignedOut notification, CancellationToken cancellationToken)
  {
    SessionEntity session = await _context.Sessions
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    session.SignOut(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
