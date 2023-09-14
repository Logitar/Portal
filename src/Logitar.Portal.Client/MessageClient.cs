using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Client;

internal class MessageClient : ClientBase, IMessageService
{
  private const string Path = "/api/messages";

  public MessageClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SentMessages>($"{Path}/send", payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }
}
