namespace Logitar.Portal.Contracts.Configurations;

public record LoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; set; }
  public bool OnlyErrors { get; set; }

  public LoggingSettings() : this(LoggingExtent.ActivityOnly, onlyErrors: false)
  {
  }

  public LoggingSettings(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }

  public LoggingSettings(LoggingExtent extent, bool onlyErrors)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
  }
}
