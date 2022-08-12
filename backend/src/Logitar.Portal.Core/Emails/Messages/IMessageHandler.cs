namespace Logitar.Portal.Core.Emails.Messages
{
  public interface IMessageHandler
  {
    Task<SendMessageResult> SendAsync(Message message, CancellationToken cancellationToken = default);
  }
}
