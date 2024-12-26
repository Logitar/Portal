using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public interface ISenderQuerier
{
  Task<SenderModel> ReadAsync(RealmModel? realm, Sender sender, CancellationToken cancellationToken = default);
  Task<SenderModel?> ReadAsync(RealmModel? realm, SenderId id, CancellationToken cancellationToken = default);
  Task<SenderModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SenderModel?> ReadDefaultAsync(RealmModel? realm, CancellationToken cancellationToken = default);
  Task<SearchResults<SenderModel>> SearchAsync(RealmModel? realm, SearchSendersPayload payload, CancellationToken cancellationToken = default);
}
