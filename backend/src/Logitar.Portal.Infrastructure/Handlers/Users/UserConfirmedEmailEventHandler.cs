using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserConfirmedEmailEventHandler : INotificationHandler<UserConfirmedEmailEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserConfirmedEmailEventHandler> _logger;

    public UserConfirmedEmailEventHandler(PortalContext context, ILogger<UserConfirmedEmailEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserConfirmedEmailEvent notification, CancellationToken cancellationToken)
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
          user.ConfirmEmail(notification);

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
