using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.ApiKeys;

public interface IApiKeyClient
{
  Task<ApiKey> AuthenticateAsync(AuthenticateApiKeyPayload payload, IRequestContext? context = null);
  Task<ApiKey> CreateAsync(CreateApiKeyPayload payload, IRequestContext? context = null);
  Task<ApiKey?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<ApiKey?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<ApiKey?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, IRequestContext? context = null);
  Task<ApiKey?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, IRequestContext? context = null);
}
