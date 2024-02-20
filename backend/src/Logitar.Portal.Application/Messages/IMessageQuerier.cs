using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages;

public interface IMessageQuerier
{
  Task<Message> ReadAsync(MessageAggregate message, CancellationToken cancellationToken = default);
  Task<Message?> ReadAsync(MessageId id, CancellationToken cancellationToken = default);
  Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken = default);
}
