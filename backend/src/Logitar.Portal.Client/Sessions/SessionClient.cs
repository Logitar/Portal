using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Client.Sessions;

internal class SessionClient : BaseClient, ISessionClient
{
  private const string Path = "/api/sessions";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public SessionClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<SessionModel> CreateAsync(CreateSessionPayload payload, IRequestContext? context)
  {
    return await PostAsync<SessionModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<SessionModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<SessionModel>(uri, context);
  }

  public async Task<SessionModel?> ReadAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await GetAsync<SessionModel>(uri, context);
  }

  public async Task<SessionModel> RenewAsync(RenewSessionPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/renew", UriKind.Relative);
    return await PutAsync<SessionModel>(uri, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(RenewAsync), HttpMethod.Put, uri, payload, context);
  }

  public async Task<SearchResults<SessionModel>> SearchAsync(SearchSessionsPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.UserId.HasValue)
    {
      builder.SetQuery("user_id", payload.UserId.Value.ToString());
    }
    if (payload.IsActive.HasValue)
    {
      builder.SetQuery("active", payload.IsActive.Value.ToString());
    }
    if (payload.IsPersistent.HasValue)
    {
      builder.SetQuery("persistent", payload.IsPersistent.Value.ToString());
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<SessionModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<SessionModel> SignInAsync(SignInSessionPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/sign/in", UriKind.Relative);
    return await PostAsync<SessionModel>(uri, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(SignInAsync), HttpMethod.Post, uri, payload, context);
  }

  public async Task<SessionModel?> SignOutAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/sign/out", UriKind.Relative);
    return await PatchAsync<SessionModel>(uri, content: null, context);
  }
}
