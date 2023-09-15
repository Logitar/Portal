using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record SendMailPayload
{
  public SendMailPayload(MessageAggregate message)
  {
    Contents = new ContentPayload[]
    {
      new (message.Template, message.Body)
    };
    Personalizations = new PersonalizationPayload[]
    {
      new(message.Recipients)
    };
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
