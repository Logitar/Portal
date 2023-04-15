using Logitar.Portal.Domain.Emails.Messages;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Emails.Providers.SendGrid.Payloads
{
  internal class ContentPayload
  {
    public ContentPayload()
    {
    }
    public ContentPayload(Message message)
    {
      ArgumentNullException.ThrowIfNull(message);

      Type = message.TemplateContentType;
      Value = message.Body;
    }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
  }
}
