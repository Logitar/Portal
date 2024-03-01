using Logitar.Portal.Contracts.Messages;
using MassTransit;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandFaultConsumer : IConsumer<Fault<SendMessageCommand>>
{
  private readonly TestContext _context;
  private readonly ILogger<SendMessageCommandFaultConsumer> _logger;

  public SendMessageCommandFaultConsumer(TestContext context, ILogger<SendMessageCommandFaultConsumer> logger)
  {
    _context = context;
    _logger = logger;
  }

  public Task Consume(ConsumeContext<Fault<SendMessageCommand>> context)
  {
    Fault<SendMessageCommand> fault = context.Message;
    _context.SendMessageCommand = SendMessageCommandResult.Fail(fault.Exceptions);

    string correlationId = context.CorrelationId?.ToString() ?? "<null>";
    _logger.LogError("X Test '{Name}' failed; CorrelationId={CorrelationId}.", nameof(_context.SendMessageCommand), correlationId);

    foreach (ExceptionInfo exception in fault.Exceptions)
    {
      _logger.LogError("CorrelationId={CorrelationId}, {ExceptionType}: {Message}", correlationId, exception.ExceptionType, exception.Message);
    }

    return Task.CompletedTask;
  }
}
