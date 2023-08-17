namespace Logitar.Portal.Contracts.Configurations;

public record LoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; set; } = LoggingExtent.ActivityOnly;
  public bool OnlyErrors { get; set; } = false;
}
