using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.ApiKeys;

public interface IApiKeyService
{
  Task<ApiKey> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKey> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKey?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKey?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken = default);
}
