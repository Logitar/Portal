namespace Logitar.Portal.Domain.Configurations;

public record ReadOnlyLoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; init; } = LoggingExtent.ActivityOnly;
  public bool OnlyErrors { get; init; } = false;
}
