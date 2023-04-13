using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Client.Implementations;

internal class DictionaryService : HttpService, IDictionaryService
{
  private const string BasePath = "/dictionaries";

  public DictionaryService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Dictionary> CreateAsync(CreateDictionaryInput input, CancellationToken cancellationToken)
    => await PostAsync<Dictionary>(BasePath, input, cancellationToken);

  public async Task<Dictionary> DeleteAsync(Guid id, CancellationToken cancellationToken)
    => await DeleteAsync<Dictionary>($"{BasePath}/{id}", cancellationToken);

  public async Task<Dictionary?> GetAsync(Guid? id, CancellationToken cancellationToken)
    => id.HasValue ? await GetAsync<Dictionary>($"{BasePath}/{id.Value}", cancellationToken) : null;

  public async Task<PagedList<Dictionary>> GetAsync(string? locale, string? realm,
    DictionarySort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    string query = GetQueryString(new Dictionary<string, object?>
    {
      [nameof(locale)] = locale,
      [nameof(realm)] = realm,
      [nameof(sort)] = sort,
      [nameof(isDescending)] = isDescending,
      [nameof(skip)] = skip,
      [nameof(limit)] = limit
    });

    return await GetAsync<PagedList<Dictionary>>($"{BasePath}?{query}", cancellationToken);
  }

  public async Task<Dictionary> UpdateAsync(Guid id, UpdateDictionaryInput input, CancellationToken cancellationToken)
    => await PutAsync<Dictionary>($"{BasePath}/{id}", input, cancellationToken);
}
