using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Client.Messages;

internal class MessageClient : BaseClient, IMessageClient
{
  private const string Path = "/api/messages";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public MessageClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<MessageModel?> ReadAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await GetAsync<MessageModel>(uri, context);
  }

  public async Task<SearchResults<MessageModel>> SearchAsync(SearchMessagesPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.TemplateId.HasValue)
    {
      builder.SetQuery("template_id", payload.TemplateId.Value.ToString());
    }
    if (payload.IsDemo.HasValue)
    {
      builder.SetQuery("demo", payload.IsDemo.Value.ToString());
    }
    if (payload.Status.HasValue)
    {
      builder.SetQuery("status", payload.Status.Value.ToString());
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<MessageModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<SentMessages> SendAsync(SendMessagePayload payload, IRequestContext? context)
  {
    return await PostAsync<SentMessages>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(SendAsync), HttpMethod.Post, UriPath, payload, context);
  }
}
