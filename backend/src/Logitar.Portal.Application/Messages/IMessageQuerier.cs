using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages;

public interface IMessageQuerier
{
  Task<Message> ReadAsync(Realm? realm, MessageAggregate message, CancellationToken cancellationToken = default);
  Task<Message?> ReadAsync(Realm? realm, MessageId id, CancellationToken cancellationToken = default);
  Task<Message?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Message>> SearchAsync(Realm? realm, SearchMessagesPayload payload, CancellationToken cancellationToken = default);
}
