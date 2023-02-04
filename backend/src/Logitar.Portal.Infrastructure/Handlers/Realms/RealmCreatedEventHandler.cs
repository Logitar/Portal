using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Realms
{
  internal class RealmCreatedEventHandler : INotificationHandler<RealmCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<RealmCreatedEventHandler> _logger;

    public RealmCreatedEventHandler(PortalContext context, ILogger<RealmCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(RealmCreatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
        RealmEntity realm = new(notification, actor);

        _context.Realms.Add(realm);

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
