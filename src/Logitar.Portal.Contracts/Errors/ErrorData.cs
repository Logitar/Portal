namespace Logitar.Portal.Contracts.Errors;

public record ErrorData
{
  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
}
