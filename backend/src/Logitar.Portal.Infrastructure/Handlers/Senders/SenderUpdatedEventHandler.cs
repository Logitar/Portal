using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Senders
{
  internal class SenderUpdatedEventHandler : INotificationHandler<SenderUpdatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<SenderUpdatedEventHandler> _logger;

    public SenderUpdatedEventHandler(PortalContext context, ILogger<SenderUpdatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(SenderUpdatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        SenderEntity? sender = await _context.Senders
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (sender == null)
        {
          _logger.LogError("The sender 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          sender.Update(notification);

          await _context.SaveChangesAsync(cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
