using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Senders;

namespace Logitar.Portal.v2.Core.Senders;

public interface ISenderQuerier
{
  Task<Sender> GetAsync(SenderAggregate sender, CancellationToken cancellationToken = default);
  Task<Sender?> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PagedList<Sender>> GetAsync(ProviderType? provider = null, string? realm = null, string? search = null,
    SenderSort? sort = null, bool isDescending = false, int? skip = null, int? limit = null, CancellationToken cancellationToken = default);
  Task<Sender?> GetDefaultAsync(string realm, CancellationToken cancellationToken = default);
}
