using Logitar.Identity.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
  private readonly PortalContext _context;

  public UserCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.Value, cancellationToken);
    if (actor == null)
    {
      actor = new(@event);

      _context.Actors.Add(actor);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
