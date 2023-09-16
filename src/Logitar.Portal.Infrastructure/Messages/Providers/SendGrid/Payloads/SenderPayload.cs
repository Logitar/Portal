using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record SenderPayload
{
  public SenderPayload(SenderSummary sender)
  {
    Address = sender.Address;
    DisplayName = sender.DisplayName;
  }

  [JsonPropertyName("email")]
  public string? Address { get; }

  [JsonPropertyName("name")]
  public string? DisplayName { get; }
}
