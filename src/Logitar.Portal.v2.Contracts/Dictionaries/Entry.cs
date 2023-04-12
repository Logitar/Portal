namespace Logitar.Portal.v2.Contracts.Dictionaries;

public record Entry
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
}
