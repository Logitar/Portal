using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Senders;
using Logitar.Portal.Core.Emails.Senders.Models;
using Logitar.Portal.Core.Emails.Senders.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class SenderService : HttpService, ISenderService
  {
    private const string BasePath = "/senders";

    public SenderService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<SenderModel> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken)
      => await PostAsync<SenderModel>(BasePath, payload, cancellationToken);

    public async Task<SenderModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
      => await DeleteAsync<SenderModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<SenderModel> GetAsync(Guid id, CancellationToken cancellationToken)
      => await GetAsync<SenderModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ListModel<SenderSummary>> GetAsync(ProviderType? provider, string? realm, string? search, SenderSort? sort, bool desc, int? index, int? count, CancellationToken cancellationToken)
    {
      string query = GetQueryString(new Dictionary<string, object?>
      {
        [nameof(provider)] = provider,
        [nameof(realm)] = realm,
        [nameof(search)] = search,
        [nameof(sort)] = sort,
        [nameof(desc)] = desc,
        [nameof(index)] = index,
        [nameof(count)] = count
      });

      return await GetAsync<ListModel<SenderSummary>>($"{BasePath}?{query}", cancellationToken);
    }

    public async Task<SenderModel> GetDefaultAsync(CancellationToken cancellationToken = default)
      => await GetAsync<SenderModel>($"{BasePath}/default", cancellationToken);

    public async Task<SenderModel> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default)
      => await PatchAsync<SenderModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<SenderModel> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
      => await PutAsync<SenderModel>($"{BasePath}/{id}", payload, cancellationToken);
  }
}
