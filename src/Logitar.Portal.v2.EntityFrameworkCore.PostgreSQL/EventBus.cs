using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL;

internal class EventBus : IEventBus
{
  private readonly IPublisher _publisher;

  public EventBus(IPublisher publisher)
  {
    _publisher = publisher;
  }

  public async Task PublishAsync(DomainEvent change, CancellationToken cancellationToken = default)
  {
    await _publisher.Publish(change, cancellationToken);
  }
}
