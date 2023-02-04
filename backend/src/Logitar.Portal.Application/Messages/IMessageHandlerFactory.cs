using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Messages
{
  internal interface IMessageHandlerFactory
  {
    IMessageHandler GetHandler(Sender sender);
  }
}
