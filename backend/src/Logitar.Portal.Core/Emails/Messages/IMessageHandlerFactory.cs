using Logitar.Portal.Core.Emails.Senders;

namespace Logitar.Portal.Core.Emails.Messages
{
  public interface IMessageHandlerFactory
  {
    IMessageHandler GetHandler(Sender sender);
  }
}
