using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
{
  internal record SendMailPayload
  {
    [JsonPropertyName("content")]
    public IEnumerable<ContentPayload>? Contents { get; init; }

    [JsonPropertyName("personalizations")]
    public IEnumerable<PersonalizationPayload>? Personalizations { get; init; }

    [JsonPropertyName("from")]
    public SenderPayload? Sender { get; init; }

    [JsonPropertyName("subject")]
    public string? Subject { get; init; }
  }
}
