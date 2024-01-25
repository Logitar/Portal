using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Infrastructure.Converters;

internal class JwtSecretUnitConverter : JsonConverter<JwtSecretUnit>
{
  public override JwtSecretUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return value == null ? null : new(value);
  }

  public override void Write(Utf8JsonWriter writer, JwtSecretUnit jwtSecret, JsonSerializerOptions options)
  {
    writer.WriteStringValue(jwtSecret.Value);
  }
}
