using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages;

public interface IMessageQuerier
{
  Task<MessageModel> ReadAsync(RealmModel? realm, Message message, CancellationToken cancellationToken = default);
  Task<MessageModel?> ReadAsync(RealmModel? realm, MessageId id, CancellationToken cancellationToken = default);
  Task<MessageModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<MessageModel>> SearchAsync(RealmModel? realm, SearchMessagesPayload payload, CancellationToken cancellationToken = default);
}
