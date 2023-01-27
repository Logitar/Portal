using Logitar.Portal.Core.Sessions.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Sessions
{
  internal class SessionCreatedEventHandler : INotificationHandler<SessionCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<SessionCreatedEventHandler> _logger;

    public SessionCreatedEventHandler(PortalContext context, ILogger<SessionCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(SessionCreatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        UserEntity? user = await _context.Users.SingleOrDefaultAsync(x => x.AggregateId == notification.UserId.ToString(), cancellationToken);
        if (user == null)
        {
          _logger.LogError("The user 'AggregateId={userId}' could not be found.", notification.UserId);
        }
        else
        {
          SessionEntity session = new(notification, user);

          _context.Sessions.Add(session);

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
