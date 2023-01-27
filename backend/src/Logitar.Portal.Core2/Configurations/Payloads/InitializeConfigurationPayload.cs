using Logitar.Portal.Core2.Users;

namespace Logitar.Portal.Core2.Configurations.Payloads
{
  public record InitializeConfigurationPayload
  {
    public string DefaultLocale { get; init; } = null!;
    public string JwtSecret { get; init; } = null!;

    public UsernameSettings UsernameSettings { get; init; } = null!;
    public PasswordSettings PasswordSettings { get; init; } = null!;

    public UserPayload User { get; set; } = null!;
  }
}
