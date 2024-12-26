using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Infrastructure.Converters;

internal class UniqueSlugConverter : JsonConverter<Slug>
{
  public override Slug? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Slug.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Slug uniqueSlug, JsonSerializerOptions options)
  {
    writer.WriteStringValue(uniqueSlug.Value);
  }
}
