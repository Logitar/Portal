using Logitar.Portal.Core.Actors;
using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserDeletedEventHandler : INotificationHandler<UserDeletedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserDeletedEventHandler> _logger;

    public UserDeletedEventHandler(PortalContext context, ILogger<UserDeletedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
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
          _context.Users.Remove(user);

          ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Type == ActorType.User && x.AggregateId == user.AggregateId, cancellationToken);
          if (actor == null)
          {
            _logger.LogError("The User actor 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
          }
          else
          {
            actor.Delete();
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
