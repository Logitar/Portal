using System.Security.Claims;

namespace Portal.Core.Tokens.Models
{
  public class ClaimModel
  {
    public ClaimModel(Claim claim)
    {
      Type = claim?.Type ?? throw new ArgumentNullException(nameof(claim));
      Value = claim.Value;
      ValueType = claim.ValueType;
    }

    public string Type { get; }
    public string Value { get; }
    public string? ValueType { get; }
  }
}
