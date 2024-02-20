using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Senders;

internal class SenderSendGridSettingsChangedEventHandler : INotificationHandler<SenderSendGridSettingsChangedEvent>
{
  private readonly PortalContext _context;

  public SenderSendGridSettingsChangedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SenderSendGridSettingsChangedEvent @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (sender != null)
    {
      sender.SetSendGridSettings(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
