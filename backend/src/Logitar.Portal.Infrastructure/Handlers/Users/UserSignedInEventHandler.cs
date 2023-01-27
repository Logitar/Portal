using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserSignedInEventHandler : INotificationHandler<UserSignedInEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserSignedInEventHandler> _logger;

    public UserSignedInEventHandler(PortalContext context, ILogger<UserSignedInEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserSignedInEvent notification, CancellationToken cancellationToken)
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
          user.SignIn(notification);

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
