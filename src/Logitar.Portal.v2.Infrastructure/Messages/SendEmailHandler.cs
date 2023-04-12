using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Errors;
using Logitar.Portal.v2.Core.Messages;
using MediatR;

namespace Logitar.Portal.v2.Infrastructure.Messages;

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
      Error error = new()
      {
        Severity = ErrorSeverity.Failure,
        Code = exception.GetType().Name.Remove(nameof(Exception)),
        Description = exception.Message
      };

      message.Fail(error);
    }
  }
}
