namespace Logitar.Portal.Contracts.Tokens;

public record Claim
{
  public Claim() : this(string.Empty, string.Empty)
  {
  }
  public Claim(string name, string value, string? type = null)
  {
    Name = name;
    Value = value;
    Type = type;
  }

  public string Name { get; set; }
  public string Value { get; set; }
  public string? Type { get; set; }
}
