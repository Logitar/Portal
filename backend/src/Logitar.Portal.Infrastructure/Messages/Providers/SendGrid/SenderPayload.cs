using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
{
  internal class SenderPayload
  {
    [JsonPropertyName("email")]
    public string? Address { get; init; }

    [JsonPropertyName("name")]
    public string? DisplayName { get; init; }
  }
}
