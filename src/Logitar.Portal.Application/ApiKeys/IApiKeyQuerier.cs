using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Domain.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys
{
  public interface IApiKeyQuerier
  {
    Task<ApiKey?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
    Task<PagedList<ApiKey>> GetPagedAsync(bool? isExpired = null, string? search = null,
      ApiKeySort? sort = null, bool desc = false,
      int? index = null, int? count = null,
      bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
