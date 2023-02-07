using Logitar.Portal.Application;
using Logitar.Portal.Domain.Users.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Users
{
  internal class UserUpdatedEventHandler : INotificationHandler<UserUpdatedEvent>
  {
    private readonly ICacheService _cacheService;
    private readonly PortalContext _context;
    private readonly ILogger<UserUpdatedEventHandler> _logger;

    public UserUpdatedEventHandler(ICacheService cacheService,
      PortalContext context,
      ILogger<UserUpdatedEventHandler> logger)
    {
      _cacheService = cacheService;
      _context = context;
      _logger = logger;
    }

    public async Task Handle(UserUpdatedEvent notification, CancellationToken cancellationToken)
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
          Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
          user.Update(notification, actor);

          await _context.UpdateActorsAsync(user.AggregateId, new Actor(user), cancellationToken);
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
