namespace Logitar.Portal.Contracts.Messages;

public interface IMessageService
{
  Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
}
