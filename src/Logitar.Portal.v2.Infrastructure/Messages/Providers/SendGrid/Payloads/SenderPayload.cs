using Logitar.Portal.v2.Core.Messages.Summaries;
using System.Text.Json.Serialization;

namespace Logitar.Portal.v2.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record SenderPayload
{
  public SenderPayload(SenderSummary sender)
  {
    Address = sender.EmailAddress;
    DisplayName = sender.DisplayName;
  }

  [JsonPropertyName("email")]
  public string? Address { get; }

  [JsonPropertyName("name")]
  public string? DisplayName { get; }
}
