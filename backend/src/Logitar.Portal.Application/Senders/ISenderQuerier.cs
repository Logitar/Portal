using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public interface ISenderQuerier
{
  Task<Sender> ReadAsync(Realm? realm, SenderAggregate sender, CancellationToken cancellationToken = default);
  Task<Sender?> ReadAsync(Realm? realm, SenderId id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadDefaultAsync(Realm? realm, CancellationToken cancellationToken = default);
  Task<SearchResults<Sender>> SearchAsync(Realm? realm, SearchSendersPayload payload, CancellationToken cancellationToken = default);
}
