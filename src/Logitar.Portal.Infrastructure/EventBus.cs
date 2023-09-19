using Logitar.EventSourcing;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Application.Logging;
using MediatR;

namespace Logitar.Portal.Infrastructure;

internal class EventBus : IEventBus
{
  private readonly ILoggingService _loggingService;
  private readonly IPublisher _publisher;

  public EventBus(ILoggingService loggingService, IPublisher publisher)
  {
    _loggingService = loggingService;
    _publisher = publisher;
  }

  public async Task PublishAsync(DomainEvent change, CancellationToken cancellationToken)
  {
    _loggingService.AddEvent(change);

    await _publisher.Publish(change, cancellationToken);
  }
}
