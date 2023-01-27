using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserEnabledEventHandler : INotificationHandler<UserEnabledEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserEnabledEventHandler> _logger;

    public UserEnabledEventHandler(PortalContext context, ILogger<UserEnabledEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserEnabledEvent notification, CancellationToken cancellationToken)
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
          user.Enable();

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
