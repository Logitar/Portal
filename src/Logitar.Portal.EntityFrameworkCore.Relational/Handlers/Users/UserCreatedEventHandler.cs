using Logitar.Portal.Domain.Users.Events;
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
    UserEntity? user = await _context.Users.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (user == null)
    {
      user = new(@event);

      _context.Users.Add(user);
    }

    ActorEntity? actor = await _context.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
    if (actor == null)
    {
      actor = new(@event);

      _context.Actors.Add(actor);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
