using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Infrastructure.Converters;

internal class SubjectConverter : JsonConverter<Subject>
{
  public override Subject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Subject.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Subject subject, JsonSerializerOptions options)
  {
    writer.WriteStringValue(subject.Value);
  }
}
