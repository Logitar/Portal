using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Infrastructure.Converters;

internal class DictionaryIdConverter : JsonConverter<DictionaryId>
{
  public override DictionaryId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return DictionaryId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, DictionaryId dictionaryId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(dictionaryId.AggregateId.Value);
  }
}
