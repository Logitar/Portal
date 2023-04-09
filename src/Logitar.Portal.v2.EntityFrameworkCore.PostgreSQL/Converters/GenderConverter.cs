using Logitar.Portal.v2.Core.Users;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Converters;

internal class GenderConverter : JsonConverter<Gender>
{
  public override Gender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();

    return value == null ? default : new Gender(value);
  }

  public override void Write(Utf8JsonWriter writer, Gender value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.Value);
  }
}
