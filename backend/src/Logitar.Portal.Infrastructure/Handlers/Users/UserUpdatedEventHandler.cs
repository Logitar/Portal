using Logitar.Portal.Core.Actors;
using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserUpdatedEventHandler> _logger;

    public UserUpdatedEventHandler(PortalContext context, ILogger<UserUpdatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        UserEntity? user = await _context.Users.SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.ToString(), cancellationToken);
        if (user == null)
        {
          _logger.LogError("The user 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          user.Update(notification);

          ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Type == ActorType.User && x.AggregateId == user.AggregateId, cancellationToken);
          if (actor == null)
          {
            _logger.LogError("The User actor 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
          }
          else
          {
            actor.Update(user);
          }

          await _context.SaveChangesAsync(cancellationToken);
        }
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
