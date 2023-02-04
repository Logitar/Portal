using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.ApiKeys
{
  internal class ApiKeyDeletedEventHandler : INotificationHandler<ApiKeyDeletedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<ApiKeyDeletedEventHandler> _logger;

    public ApiKeyDeletedEventHandler(PortalContext context, ILogger<ApiKeyDeletedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(ApiKeyDeletedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        ApiKeyEntity? apiKey = await _context.ApiKeys
          .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken);

        if (apiKey != null)
        {
          _context.ApiKeys.Remove(apiKey);

          ActorEntity? actor = await _context.Actors
            .SingleOrDefaultAsync(x => x.AggregateId == apiKey.AggregateId, cancellationToken);
          if (actor == null)
          {
            _logger.LogError("The API key 'AggregateId={aggregateId}' could  not be found.", apiKey.AggregateId);
          }
          else
          {
            actor.Delete();
          }

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
