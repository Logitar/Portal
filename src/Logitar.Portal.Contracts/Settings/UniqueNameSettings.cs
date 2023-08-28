namespace Logitar.Portal.Contracts.Settings;

public record UniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; }
}
