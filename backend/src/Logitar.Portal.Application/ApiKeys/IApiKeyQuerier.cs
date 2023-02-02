using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.ApiKeys
{
  public interface IApiKeyQuerier
  {
    Task<ApiKeyModel?> GetAsync(AggregateId id, CancellationToken cancellationToken = default);
    Task<ApiKeyModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<ApiKeyModel>> GetPagedAsync(DateTime? expiredOn = null, string? search = null,
      ApiKeySort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
  }
}
