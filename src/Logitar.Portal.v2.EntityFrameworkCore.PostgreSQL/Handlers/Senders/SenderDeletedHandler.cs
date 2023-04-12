using Logitar.Portal.v2.Core.Senders.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Senders;

internal class SenderDeletedHandler : INotificationHandler<SenderDeleted>
{
  private readonly PortalContext _context;

  public SenderDeletedHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SenderDeleted notification, CancellationToken cancellationToken)
  {
    SenderEntity sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity '{notification.AggregateId}' could not be found.");

    _context.Senders.Remove(sender);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
