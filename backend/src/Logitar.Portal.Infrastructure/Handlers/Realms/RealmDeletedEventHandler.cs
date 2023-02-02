using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Realms
{
  internal class RealmDeletedEventHandler : INotificationHandler<RealmDeletedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<RealmDeletedEventHandler> _logger;

    public RealmDeletedEventHandler(PortalContext context, ILogger<RealmDeletedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(RealmDeletedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        RealmEntity? realm = await _context.Realms
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (realm != null)
        {
          _context.Realms.Remove(realm);

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
