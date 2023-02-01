using Logitar.Portal.Application.Users.Payloads;

namespace Logitar.Portal.Application.Configurations.Payloads
{
  public record InitializeConfigurationPayload
  {
    public string DefaultLocale { get; init; } = string.Empty;
    public string JwtSecret { get; init; } = string.Empty;

    public UsernameSettingsPayload UsernameSettings { get; init; } = new();
    public PasswordSettingsPayload PasswordSettings { get; init; } = new();

    public UserPayload User { get; init; } = new();
  }
}
