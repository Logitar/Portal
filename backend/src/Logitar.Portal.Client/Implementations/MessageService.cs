using Logitar.Portal.Core;
using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Messages.Models;
using Logitar.Portal.Core.Emails.Messages.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class MessageService : HttpService, IMessageService
  {
    private const string BasePath = "/tokens";

    public MessageService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<MessageModel> GetAsync(Guid id, CancellationToken cancellationToken)
      => await GetAsync<MessageModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ListModel<MessageSummary>> GetAsync(bool? hasErrors, bool? isDemo, string? realm, string? search, bool? succeeded, string? template,
      MessageSort? sort, bool desc,
      int? index, int? count,
      CancellationToken cancellationToken)
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
        [nameof(desc)] = desc,
        [nameof(index)] = index,
        [nameof(count)] = count
      });

      return await GetAsync<ListModel<MessageSummary>>($"{BasePath}?{query}", cancellationToken);
    }

    public async Task<SentMessagesModel> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
      => await PostAsync<SentMessagesModel>(BasePath, payload, cancellationToken);
  }
}
