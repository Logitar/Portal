namespace Logitar.Portal.Core.Tokens.Models
{
  public class ClaimModel
  {
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? ValueType { get; set; }
  }
}
