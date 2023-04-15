using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core.Messages;
using MediatR;

namespace Logitar.Portal.Infrastructure.Messages;

internal class SendEmailHandler : INotificationHandler<SendEmail>
{
  private readonly IMessageHandlerFactory _messageHandlerFactory;

  public SendEmailHandler(IMessageHandlerFactory messageHandlerFactory)
  {
    _messageHandlerFactory = messageHandlerFactory;
  }

  public async Task Handle(SendEmail notification, CancellationToken cancellationToken)
  {
    using IMessageHandler messageHandler = _messageHandlerFactory.GetHandler(notification.Sender);

    MessageAggregate message = notification.Message;
    try
    {
      SendMessageResult result = await messageHandler.SendAsync(message, cancellationToken);

      message.Succeed(result);
    }
    catch (ErrorException exception)
    {
      message.Fail(exception.Error);
    }
    catch (Exception exception)
    {
      message.Fail(Error.From(exception));
    }
  }
}
