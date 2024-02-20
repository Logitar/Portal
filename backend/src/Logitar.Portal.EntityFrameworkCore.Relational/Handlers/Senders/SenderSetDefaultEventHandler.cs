using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Senders;

internal class SenderSetDefaultEventHandler : INotificationHandler<SenderSetDefaultEvent>
{
  private readonly PortalContext _context;

  public SenderSetDefaultEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SenderSetDefaultEvent @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (sender != null)
    {
      sender.SetDefault(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
