using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKey> ReadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
}
