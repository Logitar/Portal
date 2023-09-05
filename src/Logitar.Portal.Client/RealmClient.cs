using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Client;

internal class RealmClient : ClientBase, IRealmService
{
  private const string Path = "/api/realms";

  public RealmClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Realm> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<Realm>(Path, payload, cancellationToken);
  }

  public async Task<Realm?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await DeleteAsync<Realm>($"{Path}/{id}", cancellationToken);
  }

  public async Task<Realm?> ReadAsync(Guid? id, string? uniqueSlug, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Realm> realms = new(capacity: 2);

    if (id.HasValue)
    {
      Realm? realm = await GetAsync<Realm>($"{Path}/{id}", cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (uniqueSlug != null)
    {
      Realm? realm = await GetAsync<Realm>($"{Path}/unique-slug:{uniqueSlug}", cancellationToken);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw new TooManyResultsException<Realm>(expected: 1, actual: realms.Count);
    }

    return realms.Values.SingleOrDefault();
  }

  public async Task<Realm?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    StringBuilder path = new();

    path.Append(Path).Append('/').Append(id);

    if (version.HasValue)
    {
      path.Append("?version=").Append(version.Value);
    }

    return await PutAsync<Realm>(path.ToString(), payload, cancellationToken);
  }

  public async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<Realm>>($"{Path}/search", payload, cancellationToken);
  }

  public async Task<Realm?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    return await PatchAsync<Realm>($"{Path}/{id}", payload, cancellationToken);
  }
}
