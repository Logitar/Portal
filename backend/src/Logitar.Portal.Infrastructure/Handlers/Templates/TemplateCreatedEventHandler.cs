using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Templates
{
  internal class TemplateCreatedEventHandler : INotificationHandler<TemplateCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<TemplateCreatedEventHandler> _logger;

    public TemplateCreatedEventHandler(PortalContext context, ILogger<TemplateCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(TemplateCreatedEvent notification, CancellationToken cancellationToken)
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

        Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
        TemplateEntity template = new(notification, actor, realm);

        _context.Templates.Add(template);

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
