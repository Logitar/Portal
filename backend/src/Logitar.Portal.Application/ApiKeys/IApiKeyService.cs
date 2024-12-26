using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.ApiKeys;

public interface IApiKeyService
{
  Task<ApiKeyModel> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<SearchResults<ApiKeyModel>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken = default);
}
