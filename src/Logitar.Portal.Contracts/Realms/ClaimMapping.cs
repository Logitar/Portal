namespace Logitar.Portal.Contracts.Realms;

public record ClaimMapping
{
  public ClaimMapping() : this(string.Empty, string.Empty)
  {
  }
  public ClaimMapping(string key, string name, string? type = null)
  {
    Key = key;
    Name = name;
    Type = type;
  }

  public string Key { get; set; }
  public string Name { get; set; }
  public string? Type { get; set; }
}
