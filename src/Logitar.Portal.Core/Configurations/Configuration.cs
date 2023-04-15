using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Core.Configurations;

public record Configuration
{
  public string DefaultLocale { get; set; } = string.Empty;
  public string Secret { get; set; } = string.Empty;

  public UsernameSettings UsernameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();

  public LoggingSettings LoggingSettings { get; set; } = new();
}
