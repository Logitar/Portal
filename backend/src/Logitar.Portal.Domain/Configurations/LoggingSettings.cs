namespace Logitar.Portal.Domain.Configurations
{
  public record LoggingSettings
  {
    public LoggingExtent Extent { get; init; }
    public bool OnlyErrors { get; init; }
  }
}
