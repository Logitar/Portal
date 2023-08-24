using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Sessions;

internal class SessionCreatedEventHandler : INotificationHandler<SessionCreatedEvent>
{
  private readonly PortalContext _context;

  public SessionCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SessionCreatedEvent @event, CancellationToken cancellationToken)
  {
    SessionEntity? session = await _context.Sessions.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (session == null)
    {
      UserEntity user = await _context.Users
        .SingleOrDefaultAsync(x => x.AggregateId == @event.UserId.Value, cancellationToken)
        ?? throw new EntityNotFoundException<UserEntity>(@event.UserId);

      session = new(@event, user);

      _context.Sessions.Add(session);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
