using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Infrastructure.Converters;

internal class RealmIdConverter : JsonConverter<RealmId>
{
  public override RealmId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return RealmId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, RealmId realmId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(realmId.Value);
  }
}
