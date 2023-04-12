using Logitar.Portal.v2.Core.Users.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class UserCreatedHandler : INotificationHandler<UserCreated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public UserCreatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(UserCreated notification, CancellationToken cancellationToken)
  {
    RealmEntity realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity '{notification.RealmId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    UserEntity user = new(notification, realm, actor);

    _context.Users.Add(user);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
