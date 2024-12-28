using Logitar.Portal.Domain.Templates.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers;

internal class TemplateEvents : INotificationHandler<TemplateCreated>,
  INotificationHandler<TemplateDeleted>,
  INotificationHandler<TemplateUniqueKeyChanged>,
  INotificationHandler<TemplateUpdated>
{
  private readonly PortalContext _context;

  public TemplateEvents(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(TemplateCreated @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await _context.Templates.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);

    if (template == null)
    {
      template = new(@event);

      _context.Templates.Add(template);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(TemplateDeleted @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await _context.Templates
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);

    if (template != null)
    {
      _context.Templates.Remove(template);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(TemplateUniqueKeyChanged @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await _context.Templates
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);

    if (template != null && template.Version == (@event.Version - 1))
    {
      template.SetUniqueKey(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(TemplateUpdated @event, CancellationToken cancellationToken)
  {
    TemplateEntity? template = await _context.Templates
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);

    if (template != null && template.Version == (@event.Version - 1))
    {
      template.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
