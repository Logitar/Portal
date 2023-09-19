using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Messages;

internal class MessageSucceededEventHandler : INotificationHandler<MessageSucceededEvent>
{
  private readonly PortalContext _context;

  public MessageSucceededEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(MessageSucceededEvent @event, CancellationToken cancellationToken)
  {
    MessageEntity message = await _context.Messages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<MessageEntity>(@event.AggregateId);

    message.Succeed(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
