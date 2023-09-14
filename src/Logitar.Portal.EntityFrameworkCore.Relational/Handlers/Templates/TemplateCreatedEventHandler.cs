using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Templates;

internal class TemplateCreatedEventHandler : INotificationHandler<TemplateCreatedEvent>
{
  private readonly PortalContext _context;

  public TemplateCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(TemplateCreatedEvent @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await _context.Templates.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (template == null)
    {
      template = new(@event);

      _context.Templates.Add(template);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
