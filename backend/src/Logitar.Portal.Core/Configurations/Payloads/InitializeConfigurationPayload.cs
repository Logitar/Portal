using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Configurations.Payloads
{
  public class InitializeConfigurationPayload
  {
    public string DefaultLocale { get; set; } = null!;
    public string JwtSecret { get; set; } = null!;

    public UsernameSettings UsernameSettings { get; set; } = null!;
    public PasswordSettings PasswordSettings { get; set; } = null!;

    public UserPayload User { get; set; } = null!;
  }
}
