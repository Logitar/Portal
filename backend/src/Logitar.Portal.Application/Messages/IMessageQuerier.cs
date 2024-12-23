using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages;

public interface IMessageQuerier
{
  Task<Message> ReadAsync(RealmModel? realm, MessageAggregate message, CancellationToken cancellationToken = default);
  Task<Message?> ReadAsync(RealmModel? realm, MessageId id, CancellationToken cancellationToken = default);
  Task<Message?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Message>> SearchAsync(RealmModel? realm, SearchMessagesPayload payload, CancellationToken cancellationToken = default);
}
