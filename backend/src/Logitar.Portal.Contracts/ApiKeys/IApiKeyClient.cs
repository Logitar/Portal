using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.ApiKeys;

public interface IApiKeyClient
{
  Task<ApiKeyModel> AuthenticateAsync(AuthenticateApiKeyPayload payload, IRequestContext? context = null);
  Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, IRequestContext? context = null);
  Task<ApiKeyModel?> DeleteAsync(Guid id, IRequestContext? context = null);
  Task<ApiKeyModel?> ReadAsync(Guid id, IRequestContext? context = null);
  Task<ApiKeyModel?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version = null, IRequestContext? context = null);
  Task<SearchResults<ApiKeyModel>> SearchAsync(SearchApiKeysPayload payload, IRequestContext? context = null);
  Task<ApiKeyModel?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, IRequestContext? context = null);
}
