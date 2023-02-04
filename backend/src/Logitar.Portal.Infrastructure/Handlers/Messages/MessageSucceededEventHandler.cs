using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Messages
{
  internal class MessageSucceededEventHandler : INotificationHandler<MessageSucceededEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<MessageSucceededEventHandler> _logger;

    public MessageSucceededEventHandler(PortalContext context, ILogger<MessageSucceededEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(MessageSucceededEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        MessageEntity? message = await _context.Messages
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (message == null)
        {
          _logger.LogError("The sender 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
          message.Succeed(notification, actor);

          await _context.SaveChangesAsync(cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
