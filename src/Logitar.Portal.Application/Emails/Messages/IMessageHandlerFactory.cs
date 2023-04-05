using Logitar.Portal.Domain.Emails.Senders;

namespace Logitar.Portal.Application.Emails.Messages
{
  public interface IMessageHandlerFactory
  {
    IMessageHandler GetHandler(Sender sender);
  }
}
