using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Messages;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandConsumer : IConsumer<SendMessageCommand>
{
  private readonly IBus _bus;
  private readonly ILogger<SendMessageCommandConsumer> _logger;
  private readonly IMediator _mediator;
  private readonly IPopulateRequest _populateRequest;

  public SendMessageCommandConsumer(IBus bus, ILogger<SendMessageCommandConsumer> logger, IMediator mediator, IPopulateRequest populateRequest)
  {
    _bus = bus;
    _logger = logger;
    _mediator = mediator;
    _populateRequest = populateRequest;
  }

  public async Task Consume(ConsumeContext<SendMessageCommand> context)
  {
    Guid? correlationId = context.CorrelationId;

    SendMessageCommand command = context.Message;
    CancellationToken cancellationToken = context.CancellationToken;
    _logger.LogInformation("Consuming {CommandType} from CorrelationId '{CorrelationId}'.", command.GetType().Name, correlationId?.ToString() ?? "<null>");

    SendMessageInternalCommand request = new(command.Payload);
    await _populateRequest.ExecuteAsync(context, request);

    SentMessages sentMessages = await _mediator.Send(request, cancellationToken);
    MessagesSentEvent @event = new(sentMessages);

    await _bus.Publish(@event, c => c.CorrelationId = correlationId, cancellationToken);
    _logger.LogInformation("Published {EventType} to CorrelationId '{CorrelationId}'.", @event.GetType().Name, correlationId?.ToString() ?? "<null>");
  }
}
