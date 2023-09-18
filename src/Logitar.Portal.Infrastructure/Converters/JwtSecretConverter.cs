using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Infrastructure.Converters;

public class JwtSecretConverter : JsonConverter<JwtSecret?>
{
  public override JwtSecret? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? code = reader.GetString();

    return code == null ? null : new JwtSecret(code);
  }

  public override void Write(Utf8JsonWriter writer, JwtSecret? secret, JsonSerializerOptions options)
  {
    writer.WriteStringValue(secret?.Value);
  }
}
