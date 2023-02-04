using Logitar.Portal.Domain.Users.Events;
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
        UserEntity? user = await _context.Users
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (user != null)
        {
          _context.Users.Remove(user);

          await _context.UpdateActorsAsync(user.AggregateId, new Actor(user, isDeleted: true), cancellationToken);
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
