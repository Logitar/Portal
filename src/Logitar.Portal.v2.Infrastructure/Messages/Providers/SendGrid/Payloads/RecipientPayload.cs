using Logitar.Portal.v2.Core.Messages;
using System.Text.Json.Serialization;

namespace Logitar.Portal.v2.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record RecipientPayload
{
  public RecipientPayload(Recipient recipient)
  {
    Address = recipient.Address;
    DisplayName = recipient.DisplayName;
  }

  [JsonPropertyName("email")]
  public string? Address { get; }

  [JsonPropertyName("name")]
  public string? DisplayName { get; }
}
