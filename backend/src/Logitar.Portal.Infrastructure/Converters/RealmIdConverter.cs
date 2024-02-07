using Logitar.Identity.Domain.Realms;

namespace Logitar.Portal.Infrastructure.Converters;

internal class RealmIdConverter : JsonConverter<RealmId>
{
  public override RealmId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return RealmId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, RealmId configurationId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(configurationId.AggregateId.Value);
  }
}
