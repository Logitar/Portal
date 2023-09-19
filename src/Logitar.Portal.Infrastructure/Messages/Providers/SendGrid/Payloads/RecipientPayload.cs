using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record RecipientPayload
{
  public RecipientPayload(ReadOnlyRecipient recipient)
  {
    Address = recipient.Address;
    DisplayName = recipient.DisplayName;
  }

  [JsonPropertyName("email")]
  public string? Address { get; }

  [JsonPropertyName("name")]
  public string? DisplayName { get; }
}
