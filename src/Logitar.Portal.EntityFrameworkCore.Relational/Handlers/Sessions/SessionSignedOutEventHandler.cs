using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Sessions;

internal class SessionSignedOutEventHandler : INotificationHandler<SessionSignedOutEvent>
{
  private readonly PortalContext _context;

  public SessionSignedOutEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionSignedOutEvent @event, CancellationToken cancellationToken)
  {
    SessionEntity session = await _context.Sessions
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<SessionEntity>(@event.AggregateId);

    session.SignOut(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
