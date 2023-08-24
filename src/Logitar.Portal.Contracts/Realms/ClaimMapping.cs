namespace Logitar.Portal.Contracts.Realms;

public record ClaimMapping
{
  public string Key { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string? Type { get; set; }
}
