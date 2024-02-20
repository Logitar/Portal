using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public interface ISenderQuerier
{
  Task<Sender> ReadAsync(SenderAggregate sender, CancellationToken cancellationToken = default);
  Task<Sender?> ReadAsync(SenderId id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadDefaultAsync(CancellationToken cancellationToken = default);
  Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken = default);
}
