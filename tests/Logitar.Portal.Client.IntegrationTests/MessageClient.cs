using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Client;

internal class MessageClient : IMessageService
{
  private readonly HttpClient _client;

  public MessageClient(HttpClient client, PortalSettings settings)
  {
    _client = client;
    _client.BaseAddress = new Uri(settings.BaseUrl);

    if (settings.ApiKey != null)
    {
      _client.DefaultRequestHeaders.Add(Headers.XApiKey, settings.ApiKey);
    }
    else if (settings.BasicAuthentication != null)
    {
      string authorization = string.Join(' ', Schemes.Basic, settings.BasicAuthentication.Encode());
      _client.DefaultRequestHeaders.Add(Headers.Authorization, authorization);
    }
  }

  public Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public async Task<Message> SendDemoAsync(SendDemoMessagePayload payload, CancellationToken cancellationToken)
  {
    Uri requestUri = new("/api/messages/send/demo", UriKind.Relative);
    using HttpRequestMessage request = new(HttpMethod.Post, requestUri)
    {
      Content = JsonContent.Create(payload)
    };

    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    response.EnsureSuccessStatusCode();

    JsonSerializerOptions serializerOptions = new();
    serializerOptions.Converters.Add(new JsonStringEnumConverter());

    return await response.Content.ReadFromJsonAsync<Message>(serializerOptions, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }
}
