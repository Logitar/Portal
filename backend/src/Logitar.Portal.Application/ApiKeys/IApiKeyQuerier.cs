using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKey> ReadAsync(Realm? realm, ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(Realm? realm, ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(Realm? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<ApiKey>> SearchAsync(Realm? realm, SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
}
