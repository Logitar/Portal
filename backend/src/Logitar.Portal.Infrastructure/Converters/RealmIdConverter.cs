using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Infrastructure.Converters;

internal class RealmIdConverter : JsonConverter<RealmId>
{
  public override RealmId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    if (string.IsNullOrWhiteSpace(value))
    {
      return new RealmId();
    }
    StreamId streamId = new(value);
    return new(streamId);
  }

  public override void Write(Utf8JsonWriter writer, RealmId realmId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(realmId.Value);
  }
}
