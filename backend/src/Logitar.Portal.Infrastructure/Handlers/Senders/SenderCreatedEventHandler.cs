using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Senders
{
  internal class SenderCreatedEventHandler : INotificationHandler<SenderCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<SenderCreatedEventHandler> _logger;

    public SenderCreatedEventHandler(PortalContext context, ILogger<SenderCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(SenderCreatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        RealmEntity? realm = null;
        if (notification.RealmId.HasValue)
        {
          realm = await _context.Realms.SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value.Value, cancellationToken);
          if (realm == null)
          {
            _logger.LogError("The realm 'AggregateId={aggregateId}' could not be found.", notification.RealmId);

            return;
          }
        }

        SenderEntity sender = new(notification, realm);

        _context.Senders.Add(sender);

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
