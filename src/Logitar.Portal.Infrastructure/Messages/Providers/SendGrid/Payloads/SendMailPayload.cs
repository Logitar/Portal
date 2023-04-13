using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Messages;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record SendMailPayload
{
  public SendMailPayload(MessageAggregate message)
  {
    List<RecipientPayload> to = new(capacity: message.Recipients.Count());
    List<RecipientPayload> cc = new(to.Capacity);
    List<RecipientPayload> bcc = new(to.Capacity);
    foreach (Logitar.Portal.Core.Messages.Recipient recipient in message.Recipients)
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

    Contents = new[] { new ContentPayload(message.Template, message.Body) };
    Personalizations = new[] { new PersonalizationPayload(to, cc, bcc) };
    Sender = new SenderPayload(message.Sender);
    Subject = message.Subject;
  }

  [JsonPropertyName("content")]
  public IEnumerable<ContentPayload>? Contents { get; }

  [JsonPropertyName("personalizations")]
  public IEnumerable<PersonalizationPayload>? Personalizations { get; }

  [JsonPropertyName("from")]
  public SenderPayload? Sender { get; }

  [JsonPropertyName("subject")]
  public string? Subject { get; }
}
