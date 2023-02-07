using Logitar.Portal.Application;
using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserChangedPasswordEventHandler : INotificationHandler<UserChangedPasswordEvent>
  {
    private readonly ICacheService _cacheService;
    private readonly PortalContext _context;
    private readonly ILogger<UserChangedPasswordEventHandler> _logger;

    public UserChangedPasswordEventHandler(ICacheService cacheService,
      PortalContext context,
      ILogger<UserChangedPasswordEventHandler> logger)
    {
      _cacheService = cacheService;
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserChangedPasswordEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        UserEntity? user = await _context.Users
          .Include(x => x.Sessions)
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (user == null)
        {
          _logger.LogError("The user 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          user.ChangePassword(notification);

          await _context.SaveChangesAsync(cancellationToken);

          _cacheService.RemoveSessions(user.Sessions.Select(s => s.AggregateId));
        }
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
