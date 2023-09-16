using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages;

public interface IMessageQuerier
{
  Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken = default);
}
