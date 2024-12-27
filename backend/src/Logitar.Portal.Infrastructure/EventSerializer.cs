using Logitar.Identity.Infrastructure.Converters;
using Logitar.Portal.Infrastructure.Converters;

namespace Logitar.Portal.Infrastructure;

internal class EventSerializer : Identity.Infrastructure.EventSerializer
{
  public EventSerializer(PasswordConverter passwordConverter) : base(passwordConverter)
  {
  }

  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new ConfigurationIdConverter());
    SerializerOptions.Converters.Add(new DictionaryIdConverter());
    SerializerOptions.Converters.Add(new JwtSecretConverter());
    SerializerOptions.Converters.Add(new MessageIdConverter());
    SerializerOptions.Converters.Add(new RealmIdConverter());
    SerializerOptions.Converters.Add(new SenderIdConverter());
    SerializerOptions.Converters.Add(new SubjectConverter());
    SerializerOptions.Converters.Add(new TemplateIdConverter());
    SerializerOptions.Converters.Add(new SlugConverter());
  }
}
