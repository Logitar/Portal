using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Templates;

internal class TemplateUpdatedEventHandler : INotificationHandler<TemplateUpdatedEvent>
{
  private readonly PortalContext _context;

  public TemplateUpdatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(TemplateUpdatedEvent @event, CancellationToken cancellationToken)
  {
    TemplateEntity template = await _context.Templates
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<TemplateEntity>(@event.AggregateId);

    template.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
