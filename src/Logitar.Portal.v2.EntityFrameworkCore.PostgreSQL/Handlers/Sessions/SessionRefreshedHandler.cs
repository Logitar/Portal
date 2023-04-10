using Logitar.Portal.v2.Core.Sessions.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Sessions;

internal class SessionRefreshedHandler : INotificationHandler<SessionRefreshed>
{
  private readonly PortalContext _context;

  public SessionRefreshedHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionRefreshed notification, CancellationToken cancellationToken)
  {
    SessionEntity session = await _context.Sessions
      .Include(x => x.User)
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity '{notification.AggregateId}' could not be found.");

    session.Refresh(notification);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
