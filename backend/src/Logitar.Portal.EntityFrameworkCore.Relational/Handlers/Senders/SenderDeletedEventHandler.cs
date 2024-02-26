using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Senders;

internal class SenderDeletedEventHandler : INotificationHandler<SenderDeletedEvent>
{
  private readonly PortalContext _context;

  public SenderDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SenderDeletedEvent @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (sender != null)
    {
      _context.Senders.Remove(sender);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
