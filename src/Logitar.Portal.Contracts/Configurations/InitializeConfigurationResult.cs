using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Contracts.Configurations;

public record InitializeConfigurationResult
{
  public Configuration Configuration { get; set; } = new();
  public Session Session { get; set; } = new();
}
