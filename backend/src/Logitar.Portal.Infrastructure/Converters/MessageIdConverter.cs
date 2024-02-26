using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Converters;

internal class MessageIdConverter : JsonConverter<MessageId>
{
  public override MessageId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return MessageId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, MessageId messageId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(messageId.AggregateId.Value);
  }
}
