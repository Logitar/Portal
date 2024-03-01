using Logitar.Portal.Contracts.Messages;
using MassTransit;

namespace Logitar.Portal.Worker.Messages;

internal class MessagesSentEventConsumer : IConsumer<MessagesSentEvent>
{
  private readonly ILogger<MessagesSentEventConsumer> _logger;

  public MessagesSentEventConsumer(ILogger<MessagesSentEventConsumer> logger)
  {
    _logger = logger;
  }

  public Task Consume(ConsumeContext<MessagesSentEvent> context)
  {
    MessagesSentEvent @event = context.Message;
    SentMessages sentMessages = @event.SentMessages;

    int count = sentMessages.Ids.Count;
    if (context.CorrelationId.HasValue)
    {
      _logger.LogInformation("{Count} message(s) sent for Correlation ID '{CorrelationId}'.", count, context.CorrelationId.Value);
    }
    else
    {
      _logger.LogWarning("{Count} message(s) sent, but no Correlation ID was provided.", count);
    }

    return Task.CompletedTask;
  }
}
