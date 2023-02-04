using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure.Handlers.ApiKeys
{
  internal class ApiKeyCreatedEventHandler : INotificationHandler<ApiKeyCreatedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<ApiKeyCreatedEventHandler> _logger;

    public ApiKeyCreatedEventHandler(PortalContext context, ILogger<ApiKeyCreatedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(ApiKeyCreatedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        ApiKeyEntity apiKey = new(notification);
        ActorEntity actor = new(apiKey);

        _context.Actors.Add(actor);
        _context.ApiKeys.Add(apiKey);

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
