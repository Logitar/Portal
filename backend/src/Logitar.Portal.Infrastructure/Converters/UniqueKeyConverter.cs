﻿using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Infrastructure.Converters;

internal class UniqueKeyConverter : JsonConverter<UniqueKeyUnit>
{
  public override UniqueKeyUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return UniqueKeyUnit.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, UniqueKeyUnit uniqueKey, JsonSerializerOptions options)
  {
    writer.WriteStringValue(uniqueKey.Value);
  }
}
