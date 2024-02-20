using Logitar.EventSourcing;
using Logitar.Net.Mail;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Infrastructure.Messages.Providers;
using MediatR;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class SendEmailCommandHandler : INotificationHandler<SendEmailCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly Dictionary<SenderProvider, IProviderStrategy> _strategies = [];

  public SendEmailCommandHandler(IApplicationContext applicationContext, IEnumerable<IProviderStrategy> strategies)
  {
    _applicationContext = applicationContext;

    foreach (IProviderStrategy strategy in strategies)
    {
      _strategies[strategy.Provider] = strategy;
    }
  }

  public async Task Handle(SendEmailCommand command, CancellationToken cancellationToken)
  {
    MessageAggregate message = command.Message;
    SenderAggregate sender = command.Sender;

    if (!_strategies.TryGetValue(sender.Provider, out IProviderStrategy? strategy))
    {
      throw new SenderProviderNotSupportedException(sender.Provider);
    }

    IMessageHandler messageHandler = strategy.Execute(sender.Settings);

    ActorId actorId = _applicationContext.ActorId;
    SendMailResult result;
    try
    {
      result = await messageHandler.SendAsync(message, cancellationToken);
    }
    catch (Exception)
    {
      // TODO(fpion): ExceptionDetail?
      result = SendMailResult.Failure(); // TODO(fpion): data
    }

    Dictionary<string, string> resultData = new(capacity: result.Data.Count);
    foreach (KeyValuePair<string, object?> data in result.Data)
    {
      resultData[data.Key] = data.Value?.ToString() ?? string.Empty; // TODO(fpion): serialize
    }

    if (result.Succeeded)
    {
      message.Succeed(resultData, actorId);
    }
    else
    {
      message.Fail(resultData, actorId);
    }
  }
}
