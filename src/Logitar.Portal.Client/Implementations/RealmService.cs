using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Client.Implementations;

internal class RealmService : HttpService, IRealmService
{
  private const string BasePath = "/realms";

  public RealmService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Realm> CreateAsync(CreateRealmInput input, CancellationToken cancellationToken)
    => await PostAsync<Realm>(BasePath, input, cancellationToken);

  public async Task<Realm> DeleteAsync(Guid id, CancellationToken cancellationToken)
    => await DeleteAsync<Realm>($"{BasePath}/{id}", cancellationToken);

  public async Task<Realm?> GetAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    if (id != null && uniqueName != null)
    {
      throw new NotSupportedException($"You may only specify one of the following parameters: '{nameof(id)}', '{nameof(uniqueName)}'.");
    }
    else if (id == null && uniqueName == null)
    {
      return null;
    }

    string? idOrUniqueName = id?.ToString() ?? uniqueName;

    return await GetAsync<Realm>($"{BasePath}/{idOrUniqueName}", cancellationToken);
  }

  public async Task<PagedList<Realm>> GetAsync(string? search,
    RealmSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    string query = GetQueryString(new Dictionary<string, object?>
    {
      [nameof(search)] = search,
      [nameof(sort)] = sort,
      [nameof(isDescending)] = isDescending,
      [nameof(skip)] = skip,
      [nameof(limit)] = limit
    });

    return await GetAsync<PagedList<Realm>>($"{BasePath}?{query}", cancellationToken);
  }

  public async Task<Realm> UpdateAsync(Guid id, UpdateRealmInput input, CancellationToken cancellationToken)
    => await PutAsync<Realm>($"{BasePath}/{id}", input, cancellationToken);
}
