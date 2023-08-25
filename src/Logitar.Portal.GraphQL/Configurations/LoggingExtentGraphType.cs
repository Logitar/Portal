using GraphQL.Types;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.GraphQL.Configurations;

internal class LoggingExtentGraphType : EnumerationGraphType<LoggingExtent>
{
  public LoggingExtentGraphType()
  {
    Name = nameof(LoggingExtent);
    Description = "Represents the logging mode of the system.";

    Add(LoggingExtent.ActivityOnly, "Only requests which correspond to an activity within the system will be logged.");
    Add(LoggingExtent.Full, "All requests to the system will be logged.");
    Add(LoggingExtent.None, "No logging will be performed by the system.");
  }

  private void Add(LoggingExtent value, string description) => Add(value.ToString(), value, description);
}
