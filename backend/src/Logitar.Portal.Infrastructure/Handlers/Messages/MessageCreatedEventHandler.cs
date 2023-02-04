using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.Messages
{
  internal class MessageCreatedEventHandler : INotificationHandler<MessageCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<MessageCreatedEventHandler> _logger;

    public MessageCreatedEventHandler(PortalContext context, ILogger<MessageCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(MessageCreatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
        MessageEntity message = new(notification, actor);

        _context.Messages.Add(message);

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
