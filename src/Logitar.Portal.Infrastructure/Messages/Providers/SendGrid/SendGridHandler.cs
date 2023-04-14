using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core.Messages;
using Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;
using System.Net.Http.Json;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridHandler : IMessageHandler
{
  private static readonly Uri _requestUri = new("https://api.sendgrid.com/v3/mail/send");

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
    using HttpRequestMessage request = new(HttpMethod.Post, _requestUri)
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
      Error error = new()
      {
        Severity = ErrorSeverity.Failure,
        Code = response.StatusCode.ToString(),
        Description = "The message sending failed.",
        Data = result.Select(pair => new ErrorData
        {
          Key = pair.Key,
          Value = pair.Value
        })
      };

      throw new ErrorException(error, innerException);
    }

    return result;
  }
}
