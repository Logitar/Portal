using System.Text.Json.Serialization;

namespace Portal.Infrastructure.Emails.Providers.SendGrid.Payloads
{
  internal class PersonalizationPayload
  {
    public PersonalizationPayload()
    {
    }
    public PersonalizationPayload(IEnumerable<RecipientPayload> to, IEnumerable<RecipientPayload>? cc = null, IEnumerable<RecipientPayload>? bcc = null)
    {
      To = to?.Any() == true ? to : null;
      CC = cc?.Any() == true ? cc : null;
      Bcc = bcc?.Any() == true ? bcc : null;
    }

    [JsonPropertyName("bcc")]
    public IEnumerable<RecipientPayload>? Bcc { get; set; }

    [JsonPropertyName("cc")]
    public IEnumerable<RecipientPayload>? CC { get; set; }

    [JsonPropertyName("to")]
    public IEnumerable<RecipientPayload>? To { get; set; }
  }
}
