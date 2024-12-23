namespace Logitar.Portal.Contracts.Tokens;

public record ClaimModel
{
  public string Name { get; set; }
  public string Value { get; set; }
  public string? Type { get; set; }

  public ClaimModel() : this(string.Empty, string.Empty)
  {
  }
  public ClaimModel(string name, string value, string? type = null)
  {
    Name = name;
    Value = value;
    Type = type;
  }
}
