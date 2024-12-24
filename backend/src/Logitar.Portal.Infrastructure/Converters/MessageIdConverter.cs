using Logitar.EventSourcing;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Converters;

internal class MessageIdConverter : JsonConverter<MessageId>
{
  public override MessageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    if (string.IsNullOrWhiteSpace(value))
    {
      return new MessageId();
    }
    StreamId streamId = new(value);
    return new(streamId);
  }

  public override void Write(Utf8JsonWriter writer, MessageId messageId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(messageId.Value);
  }
}
