using GraphQL.Types;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.GraphQL.Configurations;

internal class LoggingSettingsGraphType : ObjectGraphType<LoggingSettings>
{
  public LoggingSettingsGraphType()
  {
    Name = nameof(LoggingSettings);
    Description = "Represents the settings of the logging in the system.";

    Field(x => x.Extent, type: typeof(NonNullGraphType<LoggingExtentGraphType>))
      .Description("The extent, or logging mode, of the system.");
    Field(x => x.OnlyErrors)
      .Description("A value indicating whether or not only requests with errors will be logged.");
  }
}
