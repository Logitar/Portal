using Logitar.Portal.Core.Messages.Summaries;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid.Payloads;

internal record ContentPayload
{
  public ContentPayload(TemplateSummary template, string body)
  {
    Type = template.ContentType;
    Value = body;
  }

  [JsonPropertyName("type")]
  public string? Type { get; }

  [JsonPropertyName("value")]
  public string? Value { get; }
}
