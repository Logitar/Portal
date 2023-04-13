using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class ExternalIdentifierSetHandler : INotificationHandler<ExternalIdentifierSet>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public ExternalIdentifierSetHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(ExternalIdentifierSet notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    user.SetExternalIdentifier(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
