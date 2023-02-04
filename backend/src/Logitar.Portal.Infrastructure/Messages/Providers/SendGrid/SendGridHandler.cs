using Logitar.Portal.Application.Messages;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;
using System.Net.Http.Json;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
{
  internal class SendGridHandler : IDisposable, IMessageHandler
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

    public async Task<SendMessageResult> SendAsync(Message message, CancellationToken cancellationToken)
    {
      SendMailPayload payload = GetSendMailPayload(message);
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
        throw new ErrorException(new Error(response.StatusCode.ToString(), "The message sending failed.", result), innerException);
      }

      return result;
    }

    private static SendMailPayload GetSendMailPayload(Message message)
    {
      List<RecipientPayload> to = new(capacity: message.Recipients.Count());
      List<RecipientPayload> cc = new(to.Capacity);
      List<RecipientPayload> bcc = new(to.Capacity);
      foreach (Recipient recipient in message.Recipients)
      {
        switch (recipient.Type)
        {
          case RecipientType.Bcc:
            bcc.Add(GetRecipientPayload(recipient));
            break;
          case RecipientType.CC:
            cc.Add(GetRecipientPayload(recipient));
            break;
          case RecipientType.To:
            to.Add(GetRecipientPayload(recipient));
            break;
        }
      }

      return new SendMailPayload()
      {
        Contents = new ContentPayload[]
        {
          new()
          {
            Type = message.TemplateContentType,
            Value = message.Body
          }
        },
        Personalizations = new PersonalizationPayload[]
        {
          new()
          {
            To = to.Any() ? to : null,
            CC = cc.Any() ? cc : null,
            Bcc = bcc.Any() ? bcc : null
          }
        },
        Sender = new SenderPayload
        {
          Address = message.SenderAddress,
          DisplayName = message.SenderDisplayName
        },
        Subject = message.Subject
      };
    }
    private static RecipientPayload GetRecipientPayload(Recipient recipient) => new()
    {
      Address = recipient.Address,
      DisplayName = recipient.DisplayName
    };
  }
}
