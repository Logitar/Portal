using Logitar.Portal.Domain;

namespace Logitar.Portal.Infrastructure.Converters;

public class ReadOnlyLocaleConverter : JsonConverter<ReadOnlyLocale?>
{
  public override ReadOnlyLocale? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? code = reader.GetString();

    return code == null ? null : new ReadOnlyLocale(code);
  }

  public override void Write(Utf8JsonWriter writer, ReadOnlyLocale? locale, JsonSerializerOptions options)
  {
    writer.WriteStringValue(locale?.Code);
  }
}
