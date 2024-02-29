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

  public async Task<Sender> CreateAsync(CreateSenderPayload payload, IRequestContext? context)
  {
    return await PostAsync<Sender>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<Sender?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<Sender>(uri, context);
  }

  public async Task<Sender?> ReadAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await GetAsync<Sender>(uri, context);
  }

  public async Task<Sender?> ReadDefaultAsync(IRequestContext? context)
  {
    Uri uri = new($"{Path}/default", UriKind.Relative);
    return await GetAsync<Sender>(uri, context);
  }

  public async Task<Sender?> ReplaceAsync(Guid id, ReplaceSenderPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<Sender>(uri, payload, context);
  }

  public async Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.Provider.HasValue)
    {
      builder.SetQuery("provider", payload.Provider.Value.ToString());
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<Sender>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<Sender?> SetDefaultAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/default", UriKind.Relative);
    return await PatchAsync<Sender>(uri, content: null, context);
  }

  public async Task<Sender?> UpdateAsync(Guid id, UpdateSenderPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<Sender>(uri, payload, context);
  }
}
