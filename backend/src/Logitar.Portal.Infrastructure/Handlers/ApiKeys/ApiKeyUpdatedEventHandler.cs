using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.ApiKeys
{
  internal class ApiKeyUpdatedEventHandler : INotificationHandler<ApiKeyUpdatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<ApiKeyUpdatedEventHandler> _logger;

    public ApiKeyUpdatedEventHandler(PortalContext context, ILogger<ApiKeyUpdatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(ApiKeyUpdatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        ApiKeyEntity? apiKey = await _context.ApiKeys
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (apiKey == null)
        {
          _logger.LogError("The API key 'AggregateId={aggregateId}' could not be found.", notification.AggregateId);
        }
        else
        {
          Actor actor = await _context.GetActorAsync(notification.ActorId, cancellationToken);
          apiKey.Update(notification, actor);

          await _context.UpdateActorsAsync(apiKey.AggregateId, new Actor(apiKey), cancellationToken);
          await _context.SaveChangesAsync(cancellationToken);
        }
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
