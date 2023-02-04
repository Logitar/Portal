using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages
{
  public interface IMessageHandler
  {
    Task<SendMessageResult> SendAsync(Message message, CancellationToken cancellationToken = default);
  }
}
