using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Senders;

internal class SenderMailgunSettingsChangedEventHandler : INotificationHandler<SenderMailgunSettingsChangedEvent>
{
  private readonly PortalContext _context;

  public SenderMailgunSettingsChangedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SenderMailgunSettingsChangedEvent @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (sender != null)
    {
      sender.SetMailgunSettings(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
