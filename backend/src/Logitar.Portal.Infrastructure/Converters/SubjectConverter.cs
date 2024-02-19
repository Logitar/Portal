using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Infrastructure.Converters;

internal class SubjectConverter : JsonConverter<SubjectUnit>
{
  public override SubjectUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return SubjectUnit.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, SubjectUnit subject, JsonSerializerOptions options)
  {
    writer.WriteStringValue(subject.Value);
  }
}
