using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
{
  internal record PersonalizationPayload
  {
    [JsonPropertyName("bcc")]
    public IEnumerable<RecipientPayload>? Bcc { get; init; }

    [JsonPropertyName("cc")]
    public IEnumerable<RecipientPayload>? CC { get; init; }

    [JsonPropertyName("to")]
    public IEnumerable<RecipientPayload>? To { get; init; }
  }
}
