using Logitar.Portal.Domain;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.JsonConverters
{
  public class AggregateIdConverter : JsonConverter<AggregateId>
  {
    public override AggregateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      string? value = reader.GetString();

      return new AggregateId(value ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, AggregateId id, JsonSerializerOptions options)
    {
      writer.WriteStringValue(id.Value);
    }
  }
}
