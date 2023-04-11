using Logitar.Portal.v2.Core.Senders;

namespace Logitar.Portal.v2.Core.Messages.Handlers;

internal interface IMessageHandlerFactory
{
  IMessageHandler GetHandler(SenderAggregate sender);
}
