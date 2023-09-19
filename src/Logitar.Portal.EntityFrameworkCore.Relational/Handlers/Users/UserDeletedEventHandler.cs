using Logitar.Portal.Domain.Users.Events;
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

  public async Task Handle(UserDeletedEvent @event, CancellationToken cancellationToken)
  {
    UserEntity? user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (user != null)
    {
      _context.Users.Remove(user);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
