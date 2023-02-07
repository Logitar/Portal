using Logitar.Portal.Application;
using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Sessions
{
  internal class SessionRenewedEventHandler : INotificationHandler<SessionRenewedEvent>
  {
    private readonly ICacheService _cacheService;
    private readonly PortalContext _context;
    private readonly ILogger<SessionRenewedEventHandler> _logger;

    public SessionRenewedEventHandler(ICacheService cacheService,
      PortalContext context,
      ILogger<SessionRenewedEventHandler> logger)
    {
      _cacheService = cacheService;
      _context = context;
      _logger = logger;
    }

    public async Task Handle(SessionRenewedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        SessionEntity? session = await _context.Sessions
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (session == null)
        {
          _logger.LogError("The session 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
          session.Renew(notification, actor);

          await _context.SaveChangesAsync(cancellationToken);
        }

        _cacheService.RemoveSession(notification.AggregateId);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
