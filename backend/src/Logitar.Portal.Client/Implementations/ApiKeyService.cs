using Logitar.Portal.Core;
using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.ApiKeys.Models;
using Logitar.Portal.Core.ApiKeys.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class ApiKeyService : HttpService, IApiKeyService
  {
    private const string BasePath = "/keys";

    public ApiKeyService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, CancellationToken cancellationToken)
      => await PostAsync<ApiKeyModel>(BasePath, payload, cancellationToken);

    public async Task<ApiKeyModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
      => await DeleteAsync<ApiKeyModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ApiKeyModel> GetAsync(Guid id, CancellationToken cancellationToken)
      => await GetAsync<ApiKeyModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ListModel<ApiKeySummary>> GetAsync(bool? isExpired, string? search, ApiKeySort? sort, bool desc, int? index, int? count, CancellationToken cancellationToken)
    {
      string query = GetQueryString(new Dictionary<string, object?>
      {
        [nameof(isExpired)] = isExpired,
        [nameof(search)] = search,
        [nameof(sort)] = sort,
        [nameof(desc)] = desc,
        [nameof(index)] = index,
        [nameof(count)] = count
      });

      return await GetAsync<ListModel<ApiKeySummary>>($"{BasePath}?{query}", cancellationToken);
    }

    public async Task<ApiKeyModel> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
      => await PutAsync<ApiKeyModel>($"{BasePath}/{id}", payload, cancellationToken);
  }
}
