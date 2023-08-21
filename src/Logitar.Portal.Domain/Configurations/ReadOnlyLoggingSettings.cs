using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Domain.Configurations;

public record ReadOnlyLoggingSettings
{
  public ReadOnlyLoggingSettings(LoggingExtent extent = LoggingExtent.ActivityOnly, bool onlyErrors = false)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
  }

  public LoggingExtent Extent { get; }
  public bool OnlyErrors { get; }
}
