using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Configurations.Payloads
{
  public record InitializeConfigurationPayload
  {
    public string DefaultLocale { get; set; } = string.Empty;
    public string JwtSecret { get; set; } = string.Empty;

    public LoggingSettingsPayload LoggingSettings { get; set; } = new();

    public UsernameSettingsPayload UsernameSettings { get; set; } = new();
    public PasswordSettingsPayload PasswordSettings { get; set; } = new();

    public UserPayload User { get; set; } = new();
  }
}
