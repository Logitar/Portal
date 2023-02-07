using Logitar.Portal.Application;
using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.ApiKeys
{
  internal class ApiKeyDeletedEventHandler : INotificationHandler<ApiKeyDeletedEvent>
  {
    private readonly ICacheService _cacheService;
    private readonly PortalContext _context;
    private readonly ILogger<ApiKeyDeletedEventHandler> _logger;

    public ApiKeyDeletedEventHandler(ICacheService cacheService,
      PortalContext context,
      ILogger<ApiKeyDeletedEventHandler> logger)
    {
      _cacheService = cacheService;
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

          await _context.UpdateActorsAsync(apiKey.AggregateId, new Actor(apiKey, isDeleted: true), cancellationToken);
          await _context.SaveChangesAsync(cancellationToken);
        }

        _cacheService.RemoveApiKey(notification.AggregateId);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
