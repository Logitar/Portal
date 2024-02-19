using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Templates;

internal class TemplateDeletedEventHandler : INotificationHandler<TemplateDeletedEvent>
{
  private readonly PortalContext _context;

  public TemplateDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(TemplateDeletedEvent @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await _context.Templates
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (template != null)
    {
      _context.Templates.Remove(template);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
