using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages
{
  internal interface IInternalMessageService
  {
    Task<SentMessagesModel> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
  }
}
