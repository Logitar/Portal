using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKey> ReadAsync(ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(string id, CancellationToken cancellationToken = default);
}
