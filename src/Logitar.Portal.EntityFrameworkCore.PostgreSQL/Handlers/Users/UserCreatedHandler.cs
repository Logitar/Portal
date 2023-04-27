using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Users;

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
    RealmEntity? realm = null;
    if (notification.RealmId.HasValue)
    {
      realm = await _context.Realms
        .SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The realm entity '{notification.RealmId}' could not be found.");
    }

    ActorEntity actor;
    if (notification.ActorId == notification.AggregateId)
    {
      actor = new()
      {
        Type = Contracts.Actors.ActorType.User,
        DisplayName = notification.FullName ?? notification.Username,
        Picture = notification.Picture?.ToString()
      };
    }
    else
    {
      actor = await _actorService.GetAsync(notification, cancellationToken);
    }

    UserEntity user = new(notification, realm, actor);

    _context.Users.Add(user);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
