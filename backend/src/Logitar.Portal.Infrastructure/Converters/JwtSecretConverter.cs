using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Infrastructure.Converters;

internal class JwtSecretConverter : JsonConverter<JwtSecret>
{
  public override JwtSecret? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, JwtSecret jwtSecret, JsonSerializerOptions options)
  {
    writer.WriteStringValue(jwtSecret.Value);
  }
}
