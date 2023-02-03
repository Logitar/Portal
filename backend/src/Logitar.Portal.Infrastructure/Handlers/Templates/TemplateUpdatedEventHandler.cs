using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Templates
{
  internal class TemplateUpdatedEventHandler : INotificationHandler<TemplateUpdatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<TemplateUpdatedEventHandler> _logger;

    public TemplateUpdatedEventHandler(PortalContext context, ILogger<TemplateUpdatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(TemplateUpdatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        TemplateEntity? template = await _context.Templates
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (template == null)
        {
          _logger.LogError("The template 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          template.Update(notification);

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
