using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Templates
{
  internal class TemplateDeletedEventHandler : INotificationHandler<TemplateDeletedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<TemplateDeletedEventHandler> _logger;

    public TemplateDeletedEventHandler(PortalContext context, ILogger<TemplateDeletedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(TemplateDeletedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        TemplateEntity? template = await _context.Templates
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (template != null)
        {
          _context.Templates.Remove(template);

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
