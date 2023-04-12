using Logitar.Portal.v2.Core.Senders;

namespace Logitar.Portal.v2.Infrastructure.Messages;

internal interface IMessageHandlerFactory
{
  IMessageHandler GetHandler(SenderAggregate sender);
}
