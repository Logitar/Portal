using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Users;

internal class UserIdentifierSetEventHandler : INotificationHandler<UserIdentifierSetEvent>
{
  private readonly PortalContext _context;

  public UserIdentifierSetEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(UserIdentifierSetEvent @event, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .Include(x => x.Identifiers)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<UserEntity>(@event.AggregateId);

    user.SetIdentifier(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
