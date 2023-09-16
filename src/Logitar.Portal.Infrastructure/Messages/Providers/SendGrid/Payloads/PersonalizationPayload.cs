using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record PersonalizationPayload
{
  public PersonalizationPayload(Recipients recipients)
  {
    Bcc = recipients.Bcc.Any() ? recipients.Bcc.Select(recipient => new RecipientPayload(recipient)) : null;
    CC = recipients.CC.Any() ? recipients.CC.Select(recipient => new RecipientPayload(recipient)) : null;
    To = recipients.To.Any() ? recipients.To.Select(recipient => new RecipientPayload(recipient)) : null;
  }

  [JsonPropertyName("bcc")]
  public IEnumerable<RecipientPayload>? Bcc { get; }

  [JsonPropertyName("cc")]
  public IEnumerable<RecipientPayload>? CC { get; }

  [JsonPropertyName("to")]
  public IEnumerable<RecipientPayload>? To { get; }
}
