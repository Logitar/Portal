using Logitar.Portal.Core.Emails.Messages;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Emails.Providers.SendGrid.Payloads
{
  internal class RecipientPayload
  {
    public RecipientPayload()
    {
    }
    public RecipientPayload(Recipient recipient)
    {
      ArgumentNullException.ThrowIfNull(recipient);

      Address = recipient.Address;
      DisplayName = recipient.DisplayName;
    }

    [JsonPropertyName("email")]
    public string? Address { get; set; }

    [JsonPropertyName("name")]
    public string? DisplayName { get; set; }
  }
}
