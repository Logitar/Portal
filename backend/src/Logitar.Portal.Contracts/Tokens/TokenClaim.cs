namespace Logitar.Portal.Contracts.Tokens;

public record TokenClaim
{
  public string Name { get; set; }
  public string Value { get; set; }
  public string? Type { get; set; }

  public TokenClaim() : this(string.Empty, string.Empty)
  {
  }
  public TokenClaim(string name, string value, string? type = null)
  {
    Name = name;
    Value = value;
    Type = type;
  }
}
