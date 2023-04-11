namespace Logitar.Portal.v2.Core.Messages.Handlers;

internal interface IMessageHandler
{
  Task<SendMessageResult> SendAsync(MessageAggregate message, CancellationToken cancellationToken = default);
}
