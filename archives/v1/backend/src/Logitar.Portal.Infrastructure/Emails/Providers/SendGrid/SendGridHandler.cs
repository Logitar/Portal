using Logitar.Portal.Application.Emails.Messages;
using Logitar.Portal.Core;
using Logitar.Portal.Domain.Emails.Messages;
using Logitar.Portal.Infrastructure.Emails.Providers.SendGrid.Payloads;
using System.Net.Http.Json;

namespace Logitar.Portal.Infrastructure.Emails.Providers.SendGrid
{
  internal class SendGridHandler : IDisposable, IMessageHandler
  {
    private static readonly Uri _requestUri = new("https://api.sendgrid.com/v3/mail/send");

    private readonly HttpClient _client = new();

    public SendGridHandler(SendGridSettings settings)
    {
      ArgumentNullException.ThrowIfNull(settings);

      _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
    }

    public void Dispose()
    {
      _client.Dispose();
    }

    public async Task<SendMessageResult> SendAsync(Message message, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(message);

      var payload = new SendMailPayload(message);
      using var request = new HttpRequestMessage(HttpMethod.Post, _requestUri)
      {
        Content = JsonContent.Create(payload)
      };

      using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);
      SendMessageResult result = await response.GetSendMessageResultAsync(cancellationToken);

      try
      {
        response.EnsureSuccessStatusCode();
      }
      catch (Exception innerException)
      {
        throw new ErrorException(new Error(
          code: response.StatusCode.ToString(),
          description: "The message sending failed.",
          data: result
        ), innerException);
      }

      return result;
    }
  }
}
