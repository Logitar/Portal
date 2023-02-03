using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys
{
  public interface IApiKeyService
  {
    Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<ApiKeyModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<ApiKeyModel>> GetAsync(DateTime? expiredOn = null, string? search = null,
      ApiKeySort? sort = null, bool isDescending = false, int? index = null, int? count = null, CancellationToken cancellationToken = default);
    Task<ApiKeyModel> UpdateAsync(string id, UpdateApiKeyPayload payload, CancellationToken cancellationToken = default);
  }
}
