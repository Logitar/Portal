using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.v2.Core.Configurations;

public record LoggingSettings
{
  private static readonly JsonSerializerOptions _options = new();
  static LoggingSettings() => _options.Converters.Add(new JsonStringEnumConverter());

  public LoggingExtent Extent { get; set; }
  public bool OnlyErrors { get; set; }

  public static LoggingSettings Deserialize(string json)
  {
    return JsonSerializer.Deserialize<LoggingSettings>(json, _options)
      ?? throw new InvalidOperationException($"The logging settings deserialization failed: '{json}'.");
  }

  public string Serialize() => JsonSerializer.Serialize(this, _options);
}
