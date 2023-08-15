using Logitar.Identity.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
{
  private readonly PortalContext _context;

  public UserDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors
      .SingleOrDefaultAsync(x => x.Id == notification.AggregateId.Value, cancellationToken);
    if (actor != null)
    {
      actor.Delete(notification);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
