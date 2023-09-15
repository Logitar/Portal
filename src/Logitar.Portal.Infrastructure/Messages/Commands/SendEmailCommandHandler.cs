using Logitar.Portal.Application;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Http;
using Logitar.Portal.Domain.Messages;
using MediatR;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class SendEmailCommandHandler : INotificationHandler<SendEmailCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILoggingService _loggingService;
  private readonly IMessageHandlerFactory _messageHandlerFactory;

  public SendEmailCommandHandler(IApplicationContext applicationContext, ILoggingService loggingService, IMessageHandlerFactory messageHandlerFactory)
  {
    _applicationContext = applicationContext;
    _loggingService = loggingService;
    _messageHandlerFactory = messageHandlerFactory;
  }

  public async Task Handle(SendEmailCommand command, CancellationToken cancellationToken)
  {
    IMessageHandler messageHandler = _messageHandlerFactory.GetHandler(command.Sender);

    MessageAggregate message = command.Message;
    try
    {
      SendMessageResult result = await messageHandler.SendAsync(message, cancellationToken);

      message.Succeed(result, _applicationContext.ActorId);
    }
    catch (Exception exception)
    {
      _loggingService.AddException(exception);

      SendMessageResult result = exception is HttpFailureException httpFailure
        ? httpFailure.Detail.ToSendMessageResult()
        : exception.ToSendMessageResult();

      message.Fail(result, _applicationContext.ActorId);
    }
  }
}
