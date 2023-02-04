namespace Logitar.Portal.Contracts.Tokens
{
  public record ClaimPayload
  {
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? ValueType { get; set; }
  }
}
