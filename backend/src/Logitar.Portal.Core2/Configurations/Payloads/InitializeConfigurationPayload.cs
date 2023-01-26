using Logitar.Portal.Core2.Users;

namespace Logitar.Portal.Core2.Configurations.Payloads
{
  internal record InitializeConfigurationPayload
  {
    public string DefaultLocale { get; init; } = null!;
    public string JwtSettings { get; init; } = null!;

    public UsernameSettings UsernameSettings { get; init; } = null!;
    public PasswordSettings PasswordSettings { get; init; } = null!;

    public UserPayload User { get; set; } = null!;
  }
}
