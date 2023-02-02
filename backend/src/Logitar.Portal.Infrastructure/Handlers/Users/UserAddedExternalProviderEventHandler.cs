using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserAddedExternalProviderEventHandler : INotificationHandler<UserAddedExternalProviderEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<UserAddedExternalProviderEventHandler> _logger;

    public UserAddedExternalProviderEventHandler(PortalContext context, ILogger<UserAddedExternalProviderEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserAddedExternalProviderEvent notification, CancellationToken cancellationToken)
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
          user.AddExternalProvider(notification);

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
