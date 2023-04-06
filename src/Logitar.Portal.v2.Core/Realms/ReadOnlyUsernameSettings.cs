using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Core.Realms;

internal record ReadOnlyUsernameSettings
{
  public ReadOnlyUsernameSettings()
  {
  }
  public ReadOnlyUsernameSettings(UsernameSettings usernameSettings)
  {
    AllowedCharacters = usernameSettings.AllowedCharacters;
  }

  public string? AllowedCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
}
