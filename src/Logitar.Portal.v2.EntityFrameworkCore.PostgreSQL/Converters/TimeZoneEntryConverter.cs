using Logitar.Portal.v2.Core.Users;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Converters;

internal class TimeZoneEntryConverter : JsonConverter<TimeZoneEntry>
{
  public override TimeZoneEntry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? id = reader.GetString();

    return id == null ? default : new TimeZoneEntry(id);
  }

  public override void Write(Utf8JsonWriter writer, TimeZoneEntry value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.Id);
  }
}
