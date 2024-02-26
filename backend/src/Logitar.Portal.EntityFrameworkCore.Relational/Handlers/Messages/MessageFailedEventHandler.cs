using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Realms;

internal class MessageFailedEventHandler : INotificationHandler<MessageFailedEvent>
{
  private readonly PortalContext _context;

  public MessageFailedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(MessageFailedEvent @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await _context.Messages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (message != null)
    {
      message.Fail(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
