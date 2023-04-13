namespace Logitar.Portal.Contracts.Senders;

public record Setting
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
}
