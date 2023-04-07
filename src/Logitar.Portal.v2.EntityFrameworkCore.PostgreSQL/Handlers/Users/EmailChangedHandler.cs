using Logitar.Portal.v2.Core.Users.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Users;

internal class EmailChangedHandler : INotificationHandler<EmailChanged>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public EmailChangedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(EmailChanged notification, CancellationToken cancellationToken)
  {
    UserEntity user = await _context.Users
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.AggregateId}' could not be found.");

    bool updateActor = user.EmailAddress != notification.Email?.Address;

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    user.SetEmail(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);

    if (updateActor)
    {
      await _actorService.UpdateAsync(user, cancellationToken);
    }
  }
}
