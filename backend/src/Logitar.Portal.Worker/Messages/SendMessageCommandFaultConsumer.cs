using Logitar.Portal.Contracts.Messages;
using MassTransit;

namespace Logitar.Portal.Worker.Messages;

internal class SendMessageCommandFaultConsumer : IConsumer<Fault<SendMessageCommand>>
{
  private readonly ILogger<SendMessageCommandFaultConsumer> _logger;

  public SendMessageCommandFaultConsumer(ILogger<SendMessageCommandFaultConsumer> logger)
  {
    _logger = logger;
  }

  public Task Consume(ConsumeContext<Fault<SendMessageCommand>> context)
  {
    foreach (ExceptionInfo exception in context.Message.Exceptions)
    {
      _logger.LogError("{ExceptionType}: {Message}", exception.ExceptionType, exception.Message);
    }

    return Task.CompletedTask;
  }
}
