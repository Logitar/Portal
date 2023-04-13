using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record PersonalizationPayload
{
  public PersonalizationPayload(IEnumerable<RecipientPayload> to,
    IEnumerable<RecipientPayload>? cc = null, IEnumerable<RecipientPayload>? bcc = null)
  {
    To = to?.Any() == true ? to : null;
    CC = cc?.Any() == true ? cc : null;
    Bcc = bcc?.Any() == true ? bcc : null;
  }

  [JsonPropertyName("bcc")]
  public IEnumerable<RecipientPayload>? Bcc { get; }

  [JsonPropertyName("cc")]
  public IEnumerable<RecipientPayload>? CC { get; }

  [JsonPropertyName("to")]
  public IEnumerable<RecipientPayload>? To { get; }
}
