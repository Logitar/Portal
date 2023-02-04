using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserDisabledEventHandler : INotificationHandler<UserDisabledEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserDisabledEventHandler> _logger;

    public UserDisabledEventHandler(PortalContext context, ILogger<UserDisabledEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserDisabledEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        UserEntity? user = await _context.Users
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (user == null)
        {
          _logger.LogError("The user 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
          user.Disable(notification, actor);

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
