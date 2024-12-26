using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Messages;

public interface IMessageService
{
  Task<MessageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<MessageModel>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken = default);
  Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
}
