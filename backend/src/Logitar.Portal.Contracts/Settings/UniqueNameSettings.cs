using Logitar.Identity.Contracts.Settings;

namespace Logitar.Portal.Contracts.Settings;

public record UniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; }

  public UniqueNameSettings() : this("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+")
  {
  }

  public UniqueNameSettings(IUniqueNameSettings uniqueName) : this(uniqueName.AllowedCharacters)
  {
  }

  public UniqueNameSettings(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
  }
}
