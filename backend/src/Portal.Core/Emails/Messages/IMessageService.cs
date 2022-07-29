using Portal.Core.Emails.Messages.Payloads;

namespace Portal.Core.Emails.Messages
{
  public interface IMessageService
  {
    Task SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
  }
}
