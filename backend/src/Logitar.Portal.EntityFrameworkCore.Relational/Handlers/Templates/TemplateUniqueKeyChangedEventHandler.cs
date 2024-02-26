using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Templates;

internal class TemplateUniqueKeyChangedEventHandler : INotificationHandler<TemplateUniqueKeyChangedEvent>
{
  private readonly PortalContext _context;

  public TemplateUniqueKeyChangedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(TemplateUniqueKeyChangedEvent @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await _context.Templates
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (template != null)
    {
      template.SetUniqueKey(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
