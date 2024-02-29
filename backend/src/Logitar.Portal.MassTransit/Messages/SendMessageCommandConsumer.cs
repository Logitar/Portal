using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Messages;
using MassTransit;
using MediatR;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandConsumer : IConsumer<SendMessageCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;
  private readonly IPopulateRequest _populateRequest;

  public SendMessageCommandConsumer(IBus bus, IMediator mediator, IPopulateRequest populateRequest)
  {
    _bus = bus;
    _mediator = mediator;
    _populateRequest = populateRequest;
  }

  public async Task Consume(ConsumeContext<SendMessageCommand> context)
  {
    SendMessageCommand command = context.Message;
    CancellationToken cancellationToken = context.CancellationToken;

    SendMessageInternalCommand request = new(command.Payload);
    await _populateRequest.ExecuteAsync(context, request);

    SentMessages sentMessages = await _mediator.Send(request, cancellationToken);
    MessagesSentEvent @event = new(sentMessages);

    await _bus.Publish(@event, c => c.CorrelationId = context.CorrelationId, cancellationToken);
  }
}
