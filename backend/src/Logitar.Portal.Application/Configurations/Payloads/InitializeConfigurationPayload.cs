using Logitar.Portal.Contracts.Users;
using System.Globalization;

namespace Logitar.Portal.Application.Configurations.Payloads
{
  public record InitializeConfigurationPayload
  {
    public CultureInfo DefaultLocale { get; init; } = CultureInfo.InvariantCulture;
    public string JwtSecret { get; init; } = string.Empty;

    public UsernameSettingsPayload UsernameSettings { get; init; } = new();
    public PasswordSettingsPayload PasswordSettings { get; init; } = new();

    public UserPayload User { get; init; } = new();
  }
}
