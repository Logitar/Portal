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

  public async Task<ApiKeyModel> AuthenticateAsync(AuthenticateApiKeyPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/authenticate", UriKind.Relative);
    return await PatchAsync<ApiKeyModel>(uri, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(AuthenticateAsync), HttpMethod.Patch, uri, payload, context);
  }

  public async Task<ApiKeyModel> CreateAsync(CreateApiKeyPayload payload, IRequestContext? context)
  {
    return await PostAsync<ApiKeyModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<ApiKeyModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<ApiKeyModel>(uri, context);
  }

  public async Task<ApiKeyModel?> ReadAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await GetAsync<ApiKeyModel>(uri, context);
  }

  public async Task<ApiKeyModel?> ReplaceAsync(Guid id, ReplaceApiKeyPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<ApiKeyModel>(uri, payload, context);
  }

  public async Task<SearchResults<ApiKeyModel>> SearchAsync(SearchApiKeysPayload payload, IRequestContext? context)
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

    return await GetAsync<SearchResults<ApiKeyModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<ApiKeyModel?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<ApiKeyModel>(uri, payload, context);
  }
}
