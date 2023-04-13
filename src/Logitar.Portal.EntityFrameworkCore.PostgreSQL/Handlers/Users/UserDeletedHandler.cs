using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class UserDeletedHandler : INotificationHandler<UserDeleted>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public UserDeletedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(UserDeleted notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.AggregateId}' could not be found.");

    _context.Users.Remove(user);
    await _context.SaveChangesAsync(cancellationToken);

    await _actorService.DeleteAsync(user, cancellationToken);
  }
}
