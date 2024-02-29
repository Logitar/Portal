using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Messages;
using MassTransit;
using MediatR;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandConsumer : IConsumer<SendMessageCommand>
{
  private readonly IBus _bus;
  private readonly IMediator _mediator;

  public SendMessageCommandConsumer(IBus bus, IMediator mediator)
  {
    _bus = bus;
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<SendMessageCommand> context)
  {
    SendMessageCommand command = context.Message;
    CancellationToken cancellationToken = context.CancellationToken;

    SendMessageInternalCommand request = new(command.Payload);
    //request.Populate(actor, configuration, realm); // TODO(fpion): populate request

    SentMessages sentMessages = await _mediator.Send(request, cancellationToken);
    MessagesSentEvent @event = new(sentMessages);

    await _bus.Publish(@event, c => c.CorrelationId = context.CorrelationId, cancellationToken);
  }
}
