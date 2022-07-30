using Portal.Core.Emails.Messages;
using System.Text.Json.Serialization;

namespace Portal.Infrastructure.Emails.Providers.SendGrid.Payloads
{
  internal class SenderPayload
  {
    public SenderPayload()
    {
    }
    public SenderPayload(Message message)
    {
      ArgumentNullException.ThrowIfNull(message);

      Address = message.SenderAddress;
      DisplayName = message.SenderDisplayName;
    }

    [JsonPropertyName("email")]
    public string? Address { get; set; }

    [JsonPropertyName("name")]
    public string? DisplayName { get; set; }
  }
}
