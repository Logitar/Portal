using Logitar.EventSourcing;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Infrastructure.Converters;

internal class SenderIdConverter : JsonConverter<SenderId>
{
  public override SenderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    if (string.IsNullOrWhiteSpace(value))
    {
      return new SenderId();
    }
    StreamId streamId = new(value);
    return new(streamId);
  }

  public override void Write(Utf8JsonWriter writer, SenderId senderId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(senderId.Value);
  }
}
