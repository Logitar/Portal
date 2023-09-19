using Logitar.Portal.Application;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Http;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Infrastructure.Messages.Providers;
using MediatR;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class SendEmailCommandHandler : INotificationHandler<SendEmailCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILoggingService _loggingService;
  private readonly Dictionary<ProviderType, IProviderStrategy> _strategies;

  public SendEmailCommandHandler(IApplicationContext applicationContext,
    ILoggingService loggingService, IEnumerable<IProviderStrategy> strategies)
  {
    _applicationContext = applicationContext;
    _loggingService = loggingService;
    _strategies = strategies.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.Last());
  }

  public async Task Handle(SendEmailCommand command, CancellationToken cancellationToken)
  {
    MessageAggregate message = command.Message;
    SenderAggregate sender = command.Sender;

    if (!_strategies.TryGetValue(sender.Provider, out IProviderStrategy? strategy))
    {
      throw new ProviderStrategyNotSupportedException(sender.Provider);
    }

    IMessageHandler messageHandler = strategy.Execute(sender.Settings);

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
