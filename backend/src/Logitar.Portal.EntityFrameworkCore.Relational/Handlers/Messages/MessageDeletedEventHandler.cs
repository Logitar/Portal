using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Messages;

internal class MessageDeletedEventHandler : INotificationHandler<MessageDeletedEvent>
{
  private readonly PortalContext _context;

  public MessageDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(MessageDeletedEvent @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await _context.Messages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (message != null)
    {
      _context.Messages.Remove(message);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
