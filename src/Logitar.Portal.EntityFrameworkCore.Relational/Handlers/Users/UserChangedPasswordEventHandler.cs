using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserChangedPasswordEventHandler : INotificationHandler<UserChangedPasswordEvent>
{
  private readonly PortalContext _context;

  public UserChangedPasswordEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(UserChangedPasswordEvent @event, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<UserEntity>(@event.AggregateId);

    user.ChangePassword(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
