using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Infrastructure.JsonConverters
{
  public class CultureInfoConverter : JsonConverter<CultureInfo>
  {
    public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      string? name = reader.GetString();

      return CultureInfo.GetCultureInfo(name ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, CultureInfo culture, JsonSerializerOptions options)
    {
      writer.WriteStringValue(culture.Name);
    }
  }
}
