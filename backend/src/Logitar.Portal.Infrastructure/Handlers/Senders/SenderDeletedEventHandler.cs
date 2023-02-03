using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Senders
{
  internal class SenderDeletedEventHandler : INotificationHandler<SenderDeletedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<SenderDeletedEventHandler> _logger;

    public SenderDeletedEventHandler(PortalContext context, ILogger<SenderDeletedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(SenderDeletedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        SenderEntity? sender = await _context.Senders
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (sender != null)
        {
          _context.Senders.Remove(sender);

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
