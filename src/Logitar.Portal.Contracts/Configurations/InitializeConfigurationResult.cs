using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Configurations;

public record InitializeConfigurationResult
{
  public Configuration Configuration { get; set; } = new();
  public User User { get; set; } = new();
}
