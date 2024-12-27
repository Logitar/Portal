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

  public async Task<RealmModel> CreateAsync(CreateRealmPayload payload, IRequestContext? context)
  {
    return await PostAsync<RealmModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<RealmModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<RealmModel>(uri, context);
  }

  public async Task<RealmModel?> ReadAsync(Guid? id, string? uniqueSlug, IRequestContext? context)
  {
    Dictionary<Guid, RealmModel> realms = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      RealmModel? realm = await GetAsync<RealmModel>(uri, context);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueSlug))
    {
      Uri uri = new($"{Path}/unique-slug:{uniqueSlug}", UriKind.Relative);
      RealmModel? realm = await GetAsync<RealmModel>(uri, context);
      if (realm != null)
      {
        realms[realm.Id] = realm;
      }
    }

    if (realms.Count > 1)
    {
      throw TooManyResultsException<RealmModel>.ExpectedSingle(realms.Count);
    }

    return realms.Values.SingleOrDefault();
  }

  public async Task<RealmModel?> ReplaceAsync(Guid id, ReplaceRealmPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<RealmModel>(uri, payload, context);
  }

  public async Task<SearchResults<RealmModel>> SearchAsync(SearchRealmsPayload payload, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath(Path).SetQuery(payload).BuildUri(UriKind.Relative);
    return await GetAsync<SearchResults<RealmModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<RealmModel?> UpdateAsync(Guid id, UpdateRealmPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<RealmModel>(uri, payload, context);
  }
}
