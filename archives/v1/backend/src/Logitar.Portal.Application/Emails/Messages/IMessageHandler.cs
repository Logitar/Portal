using Logitar.Portal.Domain.Emails.Messages;

namespace Logitar.Portal.Application.Emails.Messages
{
  public interface IMessageHandler
  {
    Task<SendMessageResult> SendAsync(Message message, CancellationToken cancellationToken = default);
  }
}
