﻿using GraphQL.Types;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.GraphQL.Settings;

namespace Logitar.Portal.GraphQL.Configurations;

internal class ConfigurationGraphType : AggregateGraphType<ConfigurationModel>
{
  public ConfigurationGraphType() : base("Represents the configuration of the system.")
  {
    Field(x => x.DefaultLocale, type: typeof(NonNullGraphType<LocaleGraphType>))
      .Description("The default language of the system.");
    Field(x => x.Secret)
      .Description("The secret to use to sign Portal security tokens.");

    Field(x => x.UniqueNameSettings, type: typeof(NonNullGraphType<UniqueNameSettingsGraphType>))
      .Description("The settings of the unique names of Portal users.");
    Field(x => x.PasswordSettings, type: typeof(NonNullGraphType<PasswordSettingsGraphType>))
      .Description("The settings of the passwords of Portal users.");

    Field(x => x.LoggingSettings, type: typeof(NonNullGraphType<LoggingSettingsGraphType>))
      .Description("The logging settings of the system.");
  }
}
