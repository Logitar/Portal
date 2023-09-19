using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Client;

internal class MessageClient : ClientBase, IMessageService
{
  private const string Path = "/api/messages";

  public MessageClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync<Message>($"{Path}/{id}", cancellationToken);
  }

  public async Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<Message>>($"{Path}/search", payload, cancellationToken)
      ?? throw new InvalidOperationException("The results should not be null.");
  }

  public async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SentMessages>($"{Path}/send", payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public Task<Message> SendDemoAsync(SendDemoMessagePayload payload, CancellationToken cancellationToken)
  {
    throw new NotSupportedException();
  }
}
