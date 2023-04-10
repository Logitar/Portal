using Logitar.Portal.v2.Core.Users.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class UserSignedInHandler : INotificationHandler<UserSignedIn>
{
  private readonly PortalContext _context;

  public UserSignedInHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(UserSignedIn notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.AggregateId}' could not be found.");

    user.SignIn(notification);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
