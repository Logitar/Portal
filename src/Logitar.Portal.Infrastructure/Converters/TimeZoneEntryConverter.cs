﻿using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Infrastructure.Converters;

public class TimeZoneEntryConverter : JsonConverter<TimeZoneEntry?>
{
  public override TimeZoneEntry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? id = reader.GetString();

    return id == null ? null : new TimeZoneEntry(id);
  }

  public override void Write(Utf8JsonWriter writer, TimeZoneEntry? timeZone, JsonSerializerOptions options)
  {
    writer.WriteStringValue(timeZone?.Id);
  }
}
