using GraphQL.Types;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.GraphQL.Settings;

namespace Logitar.Portal.GraphQL.Configurations;

internal class ConfigurationGraphType : AggregateGraphType<Configuration>
{
  public ConfigurationGraphType() : base()
  {
    Name = nameof(Configuration);
    Description = "Represents the configuration of the system.";

    Field(x => x.DefaultLocale)
      .Description("The code (ISO 639-1) of the default locale (language) of the system.");
    Field(x => x.Secret)
      .Description("The secret to use to sign security tokens.");

    Field(x => x.UniqueNameSettings, type: typeof(NonNullGraphType<UniqueNameSettingsGraphType>))
      .Description("The settings of the unique names of Portal users.");
    Field(x => x.PasswordSettings, type: typeof(NonNullGraphType<PasswordSettingsGraphType>))
      .Description("The settings of the passwords of Portal users.");

    Field(x => x.LoggingSettings, type: typeof(NonNullGraphType<LoggingSettingsGraphType>))
      .Description("The logging settings of the system.");
  }
}
