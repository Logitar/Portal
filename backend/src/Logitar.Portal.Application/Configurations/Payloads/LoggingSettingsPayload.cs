using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Configurations.Payloads
{
  public record LoggingSettingsPayload
  {
    public LoggingExtent Extent { get; set; }
    public bool OnlyErrors { get; set; }
  }
}
