using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Messages;

public interface IMessageClient
{
  Task<MessageModel?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<SearchResults<MessageModel>> SearchAsync(SearchMessagesPayload payload, IRequestContext? context = null);
  Task<SentMessages> SendAsync(SendMessagePayload payload, IRequestContext? context = null);
}
