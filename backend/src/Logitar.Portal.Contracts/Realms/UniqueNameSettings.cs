using Logitar.Identity.Contracts.Settings;

namespace Logitar.Portal.Contracts.Realms;

public record UniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
}
