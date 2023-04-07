namespace Logitar.Portal.v2.Contracts.Realms;

public record CustomAttribute
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
}
