using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Infrastructure.Converters;

internal class TemplateIdConverter : JsonConverter<TemplateId>
{
  public override TemplateId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return TemplateId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, TemplateId templateId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(templateId.AggregateId.Value);
  }
}
