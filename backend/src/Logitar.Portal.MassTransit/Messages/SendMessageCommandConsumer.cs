using Logitar.Portal.Contracts.Messages;
using MassTransit;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandConsumer : IConsumer<SendMessageCommand>
{
  private readonly IBus _bus;
  private readonly IMessageService _messageService;

  public SendMessageCommandConsumer(IBus bus, IMessageService messageService)
  {
    _bus = bus;
    _messageService = messageService;
  }

  public async Task Consume(ConsumeContext<SendMessageCommand> context)
  {
    SendMessageCommand command = context.Message;
    CancellationToken cancellationToken = context.CancellationToken;

    SentMessages sentMessages = await _messageService.SendAsync(command.Payload, cancellationToken);

    MessagesCreatedEvent @event = new(sentMessages);
    await _bus.Publish(@event, c => c.CorrelationId = context.CorrelationId, cancellationToken);
  }
}
