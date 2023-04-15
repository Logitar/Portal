using Logitar.EventSourcing;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Logging;
using MediatR;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL;

internal class EventBus : IEventBus
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILoggingService _loggingService;
  private readonly IPublisher _publisher;

  public EventBus(IApplicationContext applicationContext,
    ILoggingService loggingService,
    IPublisher publisher)
  {
    _applicationContext = applicationContext;
    _loggingService = loggingService;
    _publisher = publisher;
  }

  public async Task PublishAsync(DomainEvent change, CancellationToken cancellationToken)
  {
    Guid? activityId = _applicationContext.ActivityId;
    await _loggingService.AddEventAsync(change, activityId, cancellationToken);

    await _publisher.Publish(change, cancellationToken);
  }
}
