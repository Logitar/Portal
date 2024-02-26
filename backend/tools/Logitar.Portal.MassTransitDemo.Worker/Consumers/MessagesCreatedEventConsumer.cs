using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.MassTransitDemo.Worker.Caching;
using MassTransit;

namespace Logitar.Portal.MassTransitDemo.Worker.Consumers;

internal class MessagesCreatedEventConsumer : IConsumer<MessagesCreatedEvent>
{
  private readonly ICacheService _cacheService;
  private readonly ILogger<MessagesCreatedEventConsumer> _logger;

  public MessagesCreatedEventConsumer(ICacheService cacheService, ILogger<MessagesCreatedEventConsumer> logger)
  {
    _cacheService = cacheService;
    _logger = logger;
  }

  public Task Consume(ConsumeContext<MessagesCreatedEvent> context)
  {
    MessagesCreatedEvent @event = context.Message;
    SentMessages sentMessages = @event.SentMessages;

    Guid? correlationId = context.CorrelationId;
    if (correlationId.HasValue)
    {
      _cacheService.SetSentMessages(correlationId.Value, sentMessages);
      _logger.LogInformation("A {EventType} has been received, containing {SentMessages} message(s) for CorrelationId={CorrelationId}.",
        nameof(MessagesCreatedEvent), sentMessages.Ids.Count, correlationId.Value);
    }
    else
    {
      _logger.LogWarning("A {EventType} has been received, containing {SentMessages} message(s), but no CorrelationId was present.",
        nameof(MessagesCreatedEvent), sentMessages.Ids.Count);
    }

    return Task.CompletedTask;
  }
}
