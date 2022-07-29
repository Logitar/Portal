using Portal.Core.Emails.Messages.Models;
using Portal.Core.Emails.Messages.Payloads;

namespace Portal.Core.Emails.Messages
{
  public interface IMessageService
  {
    Task<SentMessagesModel> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
  }
}
