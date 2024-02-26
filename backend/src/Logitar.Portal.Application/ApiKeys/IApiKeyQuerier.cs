using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKey> ReadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
}
