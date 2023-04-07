using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Core.Realms;

public record ReadOnlyUsernameSettings : IUsernameSettings
{
  public ReadOnlyUsernameSettings(UsernameSettings? usernameSettings = null)
  {
    if (usernameSettings != null)
    {
      AllowedCharacters = usernameSettings.AllowedCharacters?.CleanTrim();
    }
  }

  public string? AllowedCharacters { get; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
}
