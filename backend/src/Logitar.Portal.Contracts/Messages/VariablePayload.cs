namespace Logitar.Portal.Contracts.Messages
{
  public record VariablePayload
  {
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
  }
}
