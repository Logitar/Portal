using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserSignedInEventHandler : INotificationHandler<UserSignedInEvent>
{
  private readonly PortalContext _context;

  public UserSignedInEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(UserSignedInEvent @event, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<UserEntity>(@event.AggregateId);

    user.SignIn(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
