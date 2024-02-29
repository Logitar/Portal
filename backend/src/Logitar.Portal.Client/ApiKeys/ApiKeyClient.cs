using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Client.ApiKeys;

internal class ApiKeyClient : BaseClient, IApiKeyClient
{
  private const string Path = "/api/keys";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public ApiKeyClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<ApiKey> AuthenticateAsync(AuthenticateApiKeyPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/authenticate", UriKind.Relative);
    return await PatchAsync<ApiKey>(uri, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Patch, uri, payload, context);
  }

  public async Task<ApiKey> CreateAsync(CreateApiKeyPayload payload, IRequestContext? context)
  {
    return await PostAsync<ApiKey>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<ApiKey?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<ApiKey>(uri, context);
  }

  public async Task<ApiKey?> ReadAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await GetAsync<ApiKey>(uri, context);
  }

  public async Task<ApiKey?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<ApiKey>(uri, payload, context);
  }

  public async Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.HasAuthenticated.HasValue)
    {
      builder.SetQuery("has_authenticated", payload.HasAuthenticated.Value.ToString());
    }
    if (payload.Status != null)
    {
      builder.SetQuery("expired", payload.Status.IsExpired.ToString());

      if (payload.Status.Moment.HasValue)
      {
        builder.SetQuery("moment", payload.Status.Moment.Value.ToString());
      }
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<ApiKey>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<ApiKey?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<ApiKey>(uri, payload, context);
  }
}
