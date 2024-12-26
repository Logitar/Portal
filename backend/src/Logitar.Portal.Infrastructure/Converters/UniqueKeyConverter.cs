using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Infrastructure.Converters;

internal class UniqueKeyConverter : JsonConverter<UniqueKey>
{
  public override UniqueKey? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return UniqueKey.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, UniqueKey uniqueKey, JsonSerializerOptions options)
  {
    writer.WriteStringValue(uniqueKey.Value);
  }
}
