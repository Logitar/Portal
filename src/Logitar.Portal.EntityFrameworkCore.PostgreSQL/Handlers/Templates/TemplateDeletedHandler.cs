using Logitar.Portal.Core.Templates.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Templates;

internal class TemplateDeletedHandler : INotificationHandler<TemplateDeleted>
{
  private readonly PortalContext _context;

  public TemplateDeletedHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(TemplateDeleted notification, CancellationToken cancellationToken)
  {
    TemplateEntity sender = await _context.Templates
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The template entity '{notification.AggregateId}' could not be found.");

    _context.Templates.Remove(sender);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
