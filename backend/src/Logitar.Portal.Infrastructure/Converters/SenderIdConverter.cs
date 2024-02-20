using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Infrastructure.Converters;

internal class SenderIdConverter : JsonConverter<SenderId>
{
  public override SenderId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return SenderId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, SenderId senderId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(senderId.AggregateId.Value);
  }
}
