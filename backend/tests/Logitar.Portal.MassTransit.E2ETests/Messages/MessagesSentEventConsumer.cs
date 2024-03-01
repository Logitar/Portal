using Logitar.Portal.Contracts.Messages;
using MassTransit;

namespace Logitar.Portal.MassTransit.Messages;

internal class MessagesSentEventConsumer : IConsumer<MessagesSentEvent>
{
  private readonly TestContext _context;
  private readonly ILogger<MessagesSentEventConsumer> _logger;

  public MessagesSentEventConsumer(TestContext context, ILogger<MessagesSentEventConsumer> logger)
  {
    _context = context;
    _logger = logger;
  }

  public Task Consume(ConsumeContext<MessagesSentEvent> context)
  {
    MessagesSentEvent @event = context.Message;
    SentMessages sentMessages = @event.SentMessages;
    _context.SendMessageCommand = SendMessageCommandResult.Succeed(sentMessages);

    _logger.LogInformation("✓ Test '{Name}' succeeded; {EventType}: {Count} message(s) sent for correlation ID '{CorrelationId}'.",
      nameof(_context.SendMessageCommand),
      @event.GetType().Name,
      sentMessages.Ids.Count,
      context.CorrelationId?.ToString() ?? "<null>");

    return Task.CompletedTask;
  }
}
