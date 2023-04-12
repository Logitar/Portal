using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Core.Configurations;

public record InitializeConfigurationInput
{
  public string DefaultLocale { get; set; } = string.Empty;

  public UsernameSettings? UsernameSettings { get; set; }
  public PasswordSettings? PasswordSettings { get; set; }

  public LoggingSettings? LoggingSettings { get; set; }

  public InitialUserInput User { get; set; } = new();
}
