using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class UserUpdatedHandler : INotificationHandler<UserUpdated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public UserUpdatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(UserUpdated notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.AggregateId}' could not be found.");

    bool updateActor = user.FullName != notification.FullName || user.Picture != notification.Picture?.ToString();

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    user.Update(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);

    if (updateActor)
    {
      await _actorService.UpdateAsync(user, cancellationToken);
    }
  }
}
