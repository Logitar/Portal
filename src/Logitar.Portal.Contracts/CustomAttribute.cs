namespace Logitar.Portal.Contracts;

public record CustomAttribute
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
}
