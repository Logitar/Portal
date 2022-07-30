using Portal.Core.Emails.Senders;

namespace Portal.Core.Emails.Messages
{
  public interface IMessageHandlerFactory
  {
    IMessageHandler GetHandler(Sender sender);
  }
}
