namespace Logitar.Portal.Core.Tokens.Payloads
{
  public class ClaimPayload
  {
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? ValueType { get; set; }
  }
}
