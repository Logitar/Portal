using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Sessions;

internal class SessionDeletedEventHandler : INotificationHandler<SessionDeletedEvent>
{
  private readonly PortalContext _context;

  public SessionDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionDeletedEvent @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await _context.Sessions
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (session != null)
    {
      _context.Sessions.Remove(session);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
