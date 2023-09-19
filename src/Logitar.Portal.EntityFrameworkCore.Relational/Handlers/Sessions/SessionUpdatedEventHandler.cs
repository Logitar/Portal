using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Sessions;

internal class SessionUpdatedEventHandler : INotificationHandler<SessionUpdatedEvent>
{
  private readonly PortalContext _context;

  public SessionUpdatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionUpdatedEvent @event, CancellationToken cancellationToken)
  {
    SessionEntity session = await _context.Sessions
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<SessionEntity>(@event.AggregateId);

    session.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
