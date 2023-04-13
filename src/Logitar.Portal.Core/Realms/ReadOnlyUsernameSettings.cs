using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Core.Realms;

public record ReadOnlyUsernameSettings : IUsernameSettings
{
  public string? AllowedCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

  public static ReadOnlyUsernameSettings? From(UsernameSettings? usernameSettings)
  {
    return usernameSettings == null ? null : new()
    {
      AllowedCharacters = usernameSettings.AllowedCharacters?.CleanTrim()
    };
  }
}
