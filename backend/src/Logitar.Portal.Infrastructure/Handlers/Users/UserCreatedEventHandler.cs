using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(PortalContext context, ILogger<UserCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        RealmEntity? realm = null;
        if (notification.RealmId.HasValue)
        {
          realm = await _context.Realms.SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value.Value, cancellationToken);
          if (realm == null)
          {
            _logger.LogError("The realm 'AggregateId={aggregateId}' could not be found.", notification.RealmId);

            return;
          }
        }

        Actor actor;
        if (notification.AggregateId == notification.ActorId)
        {
          actor = new Actor
          {
            Type = ActorType.User,
            DisplayName = notification.FullName ?? notification.Username,
            Email = notification.Email,
            Picture = notification.Picture
          };
        }
        else
        {
          actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
        }
        UserEntity user = new(notification, actor, realm);

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
