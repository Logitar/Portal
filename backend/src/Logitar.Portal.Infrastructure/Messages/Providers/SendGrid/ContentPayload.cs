using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
{
  internal record ContentPayload
  {
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
  }
}
