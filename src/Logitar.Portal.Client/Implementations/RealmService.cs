using Logitar.Portal.Core;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Realms.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class RealmService : HttpService, IRealmService
  {
    private const string BasePath = "/realms";

    public RealmService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<RealmModel> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken)
      => await PostAsync<RealmModel>(BasePath, payload, cancellationToken);

    public async Task<RealmModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
      => await DeleteAsync<RealmModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<RealmModel> GetAsync(string id, CancellationToken cancellationToken)
      => await GetAsync<RealmModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ListModel<RealmSummary>> GetAsync(string? search, RealmSort? sort, bool desc, int? index, int? count, CancellationToken cancellationToken)
    {
      string query = GetQueryString(new Dictionary<string, object?>
      {
        [nameof(search)] = search,
        [nameof(sort)] = sort,
        [nameof(desc)] = desc,
        [nameof(index)] = index,
        [nameof(count)] = count
      });

      return await GetAsync<ListModel<RealmSummary>>($"{BasePath}?{query}", cancellationToken);
    }

    public async Task<RealmModel> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken)
      => await PutAsync<RealmModel>($"{BasePath}/{id}", payload, cancellationToken);
  }
}
