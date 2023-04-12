using Logitar.Portal.Core;
using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.ApiKeys.Models;
using Logitar.Portal.Core.ApiKeys.Payloads;

namespace Logitar.Portal.Client
{
  public interface IApiKeyService
  {
    Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken = default);
    Task<ApiKeyModel> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiKeyModel> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ListModel<ApiKeySummary>> GetAsync(bool? isExpired = null, string? search = null,
      ApiKeySort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<ApiKeyModel> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken = default);
  }
}
