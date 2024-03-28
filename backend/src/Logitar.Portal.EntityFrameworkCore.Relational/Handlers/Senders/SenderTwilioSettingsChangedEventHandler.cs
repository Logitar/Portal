using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Senders;

internal class SenderTwilioSettingsChangedEventHandler : INotificationHandler<SenderTwilioSettingsChangedEvent>
{
  private readonly PortalContext _context;

  public SenderTwilioSettingsChangedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(SenderTwilioSettingsChangedEvent @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (sender != null)
    {
      sender.SetTwilioSettings(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
