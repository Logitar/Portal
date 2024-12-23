using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public interface ISenderQuerier
{
  Task<Sender> ReadAsync(RealmModel? realm, SenderAggregate sender, CancellationToken cancellationToken = default);
  Task<Sender?> ReadAsync(RealmModel? realm, SenderId id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadDefaultAsync(RealmModel? realm, CancellationToken cancellationToken = default);
  Task<SearchResults<Sender>> SearchAsync(RealmModel? realm, SearchSendersPayload payload, CancellationToken cancellationToken = default);
}
