using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Domain.Emails.Messages;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Emails.Providers.SendGrid.Payloads
{
  internal class SendMailPayload
  {
    public SendMailPayload()
    {
    }
    public SendMailPayload(Message message)
    {
      ArgumentNullException.ThrowIfNull(message);

      var to = new List<RecipientPayload>(capacity: message.Recipients.Count());
      var cc = new List<RecipientPayload>(to.Capacity);
      var bcc = new List<RecipientPayload>(to.Capacity);
      foreach (Recipient recipient in message.Recipients)
      {
        switch (recipient.Type)
        {
          case RecipientType.Bcc:
            bcc.Add(new RecipientPayload(recipient));
            break;
          case RecipientType.CC:
            cc.Add(new RecipientPayload(recipient));
            break;
          case RecipientType.To:
            to.Add(new RecipientPayload(recipient));
            break;
        }
      }

      Contents = new[] { new ContentPayload(message) };
      Personalizations = new[] { new PersonalizationPayload(to, cc, bcc) };
      Sender = new SenderPayload(message);
      Subject = message.Subject;
    }

    [JsonPropertyName("content")]
    public IEnumerable<ContentPayload>? Contents { get; set; }

    [JsonPropertyName("personalizations")]
    public IEnumerable<PersonalizationPayload>? Personalizations { get; set; }

    [JsonPropertyName("from")]
    public SenderPayload? Sender { get; set; }

    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
  }
}
