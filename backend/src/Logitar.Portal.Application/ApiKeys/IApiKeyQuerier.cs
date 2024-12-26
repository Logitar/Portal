using Logitar.Identity.Domain.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKeyModel> ReadAsync(RealmModel? realm, ApiKeyAggregate apiKey, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReadAsync(RealmModel? realm, ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKeyModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<ApiKeyModel>> SearchAsync(RealmModel? realm, SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
}
