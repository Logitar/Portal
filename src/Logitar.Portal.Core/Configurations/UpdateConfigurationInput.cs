using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Core.Configurations;

public record UpdateConfigurationInput
{
  public string DefaultLocale { get; set; } = string.Empty;
  public string? Secret { get; set; }

  public UsernameSettings? UsernameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }

  public LoggingSettings? LoggingSettings { get; set; }
}
