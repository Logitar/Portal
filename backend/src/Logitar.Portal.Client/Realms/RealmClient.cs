using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Client.Realms;

internal class RealmClient : BaseClient, IRealmClient
{
  private const string Path = "/api/realms";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public RealmClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Realm> CreateAsync(CreateRealmPayload payload, IRequestContext? context)
  {
    return await PostAsync<Realm>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<Realm?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<Realm>(uri, context);
  }

  public async Task<Realm?> ReadAsync(Guid? id, string? uniqueSlug, IRequestContext? context)
  {
    Dictionary<Guid, Realm> realms = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Realm? realm = await GetAsync<Realm>(uri, context);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueSlug))
    {
      Uri uri = new($"{Path}/unique-slug:{uniqueSlug}", UriKind.Relative);
      Realm? realm = await GetAsync<Realm>(uri, context);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw new TooManyResultsException<Realm>(expectedCount: 1, actualCount: realms.Count);
    }

    return realms.Values.SingleOrDefault();
  }

  public async Task<Realm?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<Realm>(uri, payload, context);
  }

  public async Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath(Path).SetQuery(payload).BuildUri(UriKind.Relative);
    return await GetAsync<SearchResults<Realm>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<Realm?> UpdateAsync(Guid id, UpdateRealmPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<Realm>(uri, payload, context);
  }
}
