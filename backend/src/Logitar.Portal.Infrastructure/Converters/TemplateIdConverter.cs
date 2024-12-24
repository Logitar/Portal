using Logitar.EventSourcing;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Infrastructure.Converters;

internal class TemplateIdConverter : JsonConverter<TemplateId>
{
  public override TemplateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    if (string.IsNullOrWhiteSpace(value))
    {
      return new TemplateId();
    }
    StreamId streamId = new(value);
    return new(streamId);
  }

  public override void Write(Utf8JsonWriter writer, TemplateId templateId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(templateId.Value);
  }
}
