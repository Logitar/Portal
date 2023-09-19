using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Senders;

internal class SenderCreatedEventHandler : INotificationHandler<SenderCreatedEvent>
{
  private readonly PortalContext _context;

  public SenderCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SenderCreatedEvent @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (sender == null)
    {
      sender = new(@event);

      _context.Senders.Add(sender);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
