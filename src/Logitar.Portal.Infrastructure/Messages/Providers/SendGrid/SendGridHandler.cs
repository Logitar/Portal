using Logitar.Portal.Contracts.Http;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridHandler : IMessageHandler
{
  private readonly HttpClient _client = new();

  public SendGridHandler(SendGridSettings settings)
  {
    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
  }

  public void Dispose()
  {
    _client.Dispose();
  }

  public async Task<SendMessageResult> SendAsync(MessageAggregate message, CancellationToken cancellationToken)
  {
    SendMailPayload payload = new(message);
    Uri requestUri = new("https://api.sendgrid.com/v3/mail/send");
    using HttpRequestMessage request = new(HttpMethod.Post, requestUri)
    {
      Content = JsonContent.Create(payload)
    };

    using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
    HttpResponseDetail detail = await response.DetailAsync(cancellationToken);

    try
    {
      response.EnsureSuccessStatusCode();
    }
    catch (Exception innerException)
    {
      throw new HttpFailureException(detail, innerException);
    }

    return detail.ToSendMessageResult();
  }
}
