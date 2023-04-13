namespace Logitar.Portal.Contracts.Messages;

public record ResultData
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
}
