using Logitar.EventSourcing;
using Logitar.Net.Mail;
using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Infrastructure.Messages.Providers;
using MediatR;

namespace Logitar.Portal.Infrastructure.Messages.Commands;

internal class SendEmailCommandHandler : IRequestHandler<SendEmailCommand>
{
  private readonly Dictionary<SenderProvider, IProviderStrategy> _strategies = [];

  public SendEmailCommandHandler(IEnumerable<IProviderStrategy> strategies)
  {
    foreach (IProviderStrategy strategy in strategies)
    {
      _strategies[strategy.Provider] = strategy;
    }
  }

  public async Task Handle(SendEmailCommand command, CancellationToken cancellationToken)
  {
    ActorId actorId = command.ActorId;
    Message message = command.Message;
    Sender sender = command.Sender;

    if (!_strategies.TryGetValue(sender.Provider, out IProviderStrategy? strategy))
    {
      throw new SenderProviderNotSupportedException(sender.Provider);
    }

    IMessageHandler messageHandler = strategy.Execute(sender.Settings);

    SendMailResult result;
    try
    {
      result = await messageHandler.SendAsync(message, cancellationToken);
    }
    catch (Exception exception)
    {
      result = ToResult(exception);
    }

    IReadOnlyDictionary<string, string> resultData = SerializeData(result);
    if (result.Succeeded)
    {
      message.Succeed(resultData, actorId);
    }
    else
    {
      message.Fail(resultData, actorId);
    }
  }

  private static Dictionary<string, string> SerializeData(SendMailResult result)
  {
    Dictionary<string, string> resultData = new(capacity: result.Data.Count);
    foreach (KeyValuePair<string, object?> data in result.Data)
    {
      if (data.Value != null)
      {
        resultData[data.Key] = JsonSerializer.Serialize(data.Value, data.Value.GetType()).Trim('"');
      }
    }
    return resultData;
  }

  private static SendMailResult ToResult(Exception exception)
  {
    return SendMailResult.Failure(new Dictionary<string, object?>
    {
      ["Type"] = exception.GetType().GetNamespaceQualifiedName(),
      ["Message"] = exception.Message,
      ["InnerException"] = null, // TODO(fpion): implement
      ["HResult"] = exception.HResult,
      ["HelpLink"] = exception.HelpLink,
      ["Source"] = exception.Source,
      ["StackTrace"] = exception.StackTrace,
      ["TargetSite"] = exception.TargetSite?.ToString(),
      ["Data"] = exception.Data
    });
  }
}
