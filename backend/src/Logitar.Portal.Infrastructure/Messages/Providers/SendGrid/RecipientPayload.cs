using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
{
  internal record RecipientPayload
  {
    [JsonPropertyName("email")]
    public string? Address { get; init; }

    [JsonPropertyName("name")]
    public string? DisplayName { get; init; }
  }
}
