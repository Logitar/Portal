namespace Logitar.Portal.Core.Configurations;

public record LoggingSettings
{
  public LoggingExtent Extent { get; set; }
  public bool OnlyErrors { get; set; }
}
