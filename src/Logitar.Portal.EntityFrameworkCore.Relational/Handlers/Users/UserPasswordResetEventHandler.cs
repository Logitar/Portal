using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserPasswordResetEventHandler : INotificationHandler<UserPasswordResetEvent>
{
  private readonly PortalContext _context;

  public UserPasswordResetEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(UserPasswordResetEvent @event, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<UserEntity>(@event.AggregateId);

    user.ResetPassword(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
