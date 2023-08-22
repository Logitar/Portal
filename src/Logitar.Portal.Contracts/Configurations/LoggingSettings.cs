namespace Logitar.Portal.Contracts.Configurations;

public record LoggingSettings
{
  public LoggingExtent Extent { get; set; }
  public bool OnlyErrors { get; set; }
}
