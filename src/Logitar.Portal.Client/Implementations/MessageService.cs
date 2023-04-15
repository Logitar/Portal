using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Client.Implementations;

internal class MessageService : HttpService, IMessageService
{
  private const string BasePath = "/messages";

  public MessageService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Message?> GetAsync(Guid? id, CancellationToken cancellationToken)
    => id.HasValue ? await GetAsync<Message>($"{BasePath}/{id.Value}", cancellationToken) : null;

  public async Task<PagedList<Message>> GetAsync(bool? hasErrors, bool? isDemo, string? realm, string? search, bool? succeeded, string? template,
    MessageSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    string query = GetQueryString(new Dictionary<string, object?>
    {
      [nameof(hasErrors)] = hasErrors,
      [nameof(isDemo)] = isDemo,
      [nameof(realm)] = realm,
      [nameof(search)] = search,
      [nameof(succeeded)] = succeeded,
      [nameof(template)] = template,
      [nameof(sort)] = sort,
      [nameof(isDescending)] = isDescending,
      [nameof(skip)] = skip,
      [nameof(limit)] = limit
    });

    return await GetAsync<PagedList<Message>>($"{BasePath}?{query}", cancellationToken);
  }

  public async Task<SentMessages> SendAsync(SendMessageInput input, CancellationToken cancellationToken)
    => await PostAsync<SentMessages>(BasePath, input, cancellationToken);

  public Task<Message> SendDemoAsync(SendDemoMessageInput input, CancellationToken cancellationToken)
  {
    throw new NotSupportedException("You may only send demo messages from the Portal interface.");
  }
}
