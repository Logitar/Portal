using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.MassTransit.Caching;
using MassTransit;

namespace Logitar.Portal.MassTransit.Messages;

internal class MessagesSentEventConsumer : IConsumer<MessagesSentEvent>
{
  private readonly ICacheService _cacheService;

  public MessagesSentEventConsumer(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public Task Consume(ConsumeContext<MessagesSentEvent> context)
  {
    MessagesSentEvent @event = context.Message;
    SentMessages sentMessages = @event.SentMessages;

    int count = sentMessages.Ids.Count;
    if (context.CorrelationId.HasValue)
    {
      _cacheService.SetSentMessages(context.CorrelationId.Value, sentMessages);
      Console.WriteLine("{0} message(s) sent for correlation ID '{1}'.", count, context.CorrelationId.Value);
    }
    else
    {
      Console.WriteLine("{0} message(s) sent, but no correlation ID was provided.", count);
    }

    return Task.CompletedTask;
  }
}
