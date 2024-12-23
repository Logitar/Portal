using Logitar.Identity.Core.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKeyModel> ReadAsync(Realm? realm, ApiKey apiKey, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReadAsync(Realm? realm, ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<ApiKeyModel>> SearchAsync(Realm? realm, SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
}
