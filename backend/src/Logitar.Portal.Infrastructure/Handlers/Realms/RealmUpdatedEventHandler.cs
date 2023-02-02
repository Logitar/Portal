using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Realms
{
  internal class RealmUpdatedEventHandler : INotificationHandler<RealmUpdatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<RealmUpdatedEventHandler> _logger;

    public RealmUpdatedEventHandler(PortalContext context, ILogger<RealmUpdatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(RealmUpdatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        RealmEntity? realm = await _context.Realms
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (realm == null)
        {
          _logger.LogError("The user 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          realm.Update(notification);

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
