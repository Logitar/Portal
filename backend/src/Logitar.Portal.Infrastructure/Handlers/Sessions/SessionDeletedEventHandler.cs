using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Sessions
{
  internal class SessionDeletedEventHandler : INotificationHandler<SessionDeletedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<SessionDeletedEventHandler> _logger;

    public SessionDeletedEventHandler(PortalContext context, ILogger<SessionDeletedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(SessionDeletedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        SessionEntity? session = await _context.Sessions
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (session != null)
        {
          _context.Sessions.Remove(session);

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
