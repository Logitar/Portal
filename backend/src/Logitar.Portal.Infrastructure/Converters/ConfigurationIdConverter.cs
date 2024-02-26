using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Infrastructure.Converters;

internal class ConfigurationIdConverter : JsonConverter<ConfigurationId>
{
  public override ConfigurationId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return value == null ? null : new ConfigurationId();
  }

  public override void Write(Utf8JsonWriter writer, ConfigurationId configurationId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(configurationId.AggregateId.Value);
  }
}
