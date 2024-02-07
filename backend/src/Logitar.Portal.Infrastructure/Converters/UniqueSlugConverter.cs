using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Infrastructure.Converters;

internal class UniqueSlugConverter : JsonConverter<UniqueSlugUnit>
{
  public override UniqueSlugUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return UniqueSlugUnit.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, UniqueSlugUnit uniqueSlug, JsonSerializerOptions options)
  {
    writer.WriteStringValue(uniqueSlug.Value);
  }
}
