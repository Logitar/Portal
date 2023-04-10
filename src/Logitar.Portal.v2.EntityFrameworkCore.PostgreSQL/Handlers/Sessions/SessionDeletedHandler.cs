using Logitar.Portal.v2.Core.Sessions.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Sessions;

internal class SessionDeletedHandler : INotificationHandler<SessionDeleted>
{
  private readonly PortalContext _context;

  public SessionDeletedHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionDeleted notification, CancellationToken cancellationToken)
  {
    SessionEntity session = await _context.Sessions
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity '{notification.AggregateId}' could not be found.");

    _context.Sessions.Remove(session);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
