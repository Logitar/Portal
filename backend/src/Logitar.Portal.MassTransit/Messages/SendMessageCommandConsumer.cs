using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandConsumer : IConsumer<SendMessageCommand>
{
  private const string OperationType = nameof(MassTransit);

  private readonly IBus _bus;
  private readonly ILogger<SendMessageCommandConsumer> _logger;
  private readonly IConsumerPipeline _pipeline;

  public SendMessageCommandConsumer(IBus bus, ILogger<SendMessageCommandConsumer> logger, IConsumerPipeline pipeline)
  {
    _bus = bus;
    _logger = logger;
    _pipeline = pipeline;
  }

  public async Task Consume(ConsumeContext<SendMessageCommand> context)
  {
    Guid? correlationId = context.CorrelationId;
    string correlationIdString = correlationId?.ToString() ?? "<null>";

    SendMessageCommand command = context.Message;
    CancellationToken cancellationToken = context.CancellationToken;
    _logger.LogInformation("Consuming {CommandType} from CorrelationId '{CorrelationId}'.", command.GetType().Name, correlationIdString);

    try
    {
      SendMessageInternalCommand request = new(command.Payload);
      SentMessages sentMessages = await _pipeline.ExecuteAsync(request, GetType(), context);
      MessagesSentEvent @event = new(sentMessages);
      await _bus.Publish(@event, c => c.CorrelationId = correlationId, cancellationToken);
      _logger.LogInformation("Published {EventType} to CorrelationId '{CorrelationId}'.", @event.GetType().Name, correlationIdString);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Exception '{ExceptionType}' occurred for CorrelationId '{CorrelationId}'.", exception.GetType().Name, correlationIdString);

      throw;
    }
  }
}
