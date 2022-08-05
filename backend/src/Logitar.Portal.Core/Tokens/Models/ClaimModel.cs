using System.Security.Claims;

namespace Logitar.Portal.Core.Tokens.Models
{
  public class ClaimModel
  {
    /// <summary>
    /// Empty constructor for deserialization
    /// </summary>
    public ClaimModel()
    {
      Type = string.Empty;
      Value = string.Empty;
    }
    public ClaimModel(Claim claim)
    {
      Type = claim?.Type ?? throw new ArgumentNullException(nameof(claim));
      Value = claim.Value;
      ValueType = claim.ValueType;
    }

    public string Type { get; set; }
    public string Value { get; set; }
    public string? ValueType { get; set; }
  }
}
