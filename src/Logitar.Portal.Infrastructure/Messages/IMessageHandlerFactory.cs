using Logitar.Portal.Core.Senders;

namespace Logitar.Portal.Infrastructure.Messages;

internal interface IMessageHandlerFactory
{
  IMessageHandler GetHandler(SenderAggregate sender);
}
