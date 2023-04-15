using Logitar.Portal.Core;
using Logitar.Portal.Core.Dictionaries;
using Logitar.Portal.Core.Dictionaries.Models;
using Logitar.Portal.Core.Dictionaries.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class DictionaryService : HttpService, IDictionaryService
  {
    private const string BasePath = "/dictionaries";

    public DictionaryService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<DictionaryModel> CreateAsync(CreateDictionaryPayload payload, CancellationToken cancellationToken)
      => await PostAsync<DictionaryModel>(BasePath, payload, cancellationToken);

    public async Task<DictionaryModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
      => await DeleteAsync<DictionaryModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<DictionaryModel> GetAsync(Guid id, CancellationToken cancellationToken)
      => await GetAsync<DictionaryModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ListModel<DictionarySummary>> GetAsync(string? locale, string? realm, DictionarySort? sort, bool desc, int? index, int? count, CancellationToken cancellationToken)
    {
      string query = GetQueryString(new Dictionary<string, object?>
      {
        [nameof(locale)] = locale,
        [nameof(realm)] = realm,
        [nameof(sort)] = sort,
        [nameof(desc)] = desc,
        [nameof(index)] = index,
        [nameof(count)] = count
      });

      return await GetAsync<ListModel<DictionarySummary>>($"{BasePath}?{query}", cancellationToken);
    }

    public async Task<DictionaryModel> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
      => await PutAsync<DictionaryModel>($"{BasePath}/{id}", payload, cancellationToken);
  }
}
