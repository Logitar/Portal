using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Client.Senders;

internal class SenderClient : BaseClient, ISenderClient
{
  private const string Path = "/api/senders";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public SenderClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<SenderModel> CreateAsync(CreateSenderPayload payload, IRequestContext? context)
  {
    return await PostAsync<SenderModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<SenderModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<SenderModel>(uri, context);
  }

  public async Task<SenderModel?> ReadAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await GetAsync<SenderModel>(uri, context);
  }

  public async Task<SenderModel?> ReadDefaultAsync(IRequestContext? context)
  {
    Uri uri = new($"{Path}/default", UriKind.Relative);
    return await GetAsync<SenderModel>(uri, context);
  }

  public async Task<SenderModel?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<SenderModel>(uri, payload, context);
  }

  public async Task<SearchResults<SenderModel>> SearchAsync(SearchSendersPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.Provider.HasValue)
    {
      builder.SetQuery("provider", payload.Provider.Value.ToString());
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<SenderModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<SenderModel?> SetDefaultAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/default", UriKind.Relative);
    return await PatchAsync<SenderModel>(uri, content: null, context);
  }

  public async Task<SenderModel?> UpdateAsync(Guid id, UpdateSenderPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<SenderModel>(uri, payload, context);
  }
}
