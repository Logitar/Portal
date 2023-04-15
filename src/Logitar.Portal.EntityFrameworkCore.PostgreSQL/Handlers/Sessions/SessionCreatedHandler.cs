using Logitar.Portal.Core.Sessions.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Sessions;

internal class SessionCreatedHandler : INotificationHandler<SessionCreated>
{
  private readonly PortalContext _context;

  public SessionCreatedHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionCreated notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == notification.ActorId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.ActorId}' could not be found.");

    SessionEntity session = new(notification, user);

    _context.Sessions.Add(session);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
