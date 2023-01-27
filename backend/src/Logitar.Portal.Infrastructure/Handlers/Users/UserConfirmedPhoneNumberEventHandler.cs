using Logitar.Portal.Core.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserConfirmedPhoneNumberEventHandler : INotificationHandler<UserConfirmedPhoneNumberEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserConfirmedPhoneNumberEventHandler> _logger;

    public UserConfirmedPhoneNumberEventHandler(PortalContext context, ILogger<UserConfirmedPhoneNumberEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserConfirmedPhoneNumberEvent notification, CancellationToken cancellationToken)
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
          user.ConfirmPhoneNumber(notification);

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
