namespace Logitar.Portal.Contracts.Realms;

public record ClaimMappingModification
{
  public ClaimMappingModification() : this(string.Empty, null)
  {
  }
  public ClaimMappingModification(string key, string? name, string? type = null)
  {
    Key = key;
    Name = name;
    Type = type;
  }

  public string Key { get; set; }
  public string? Name { get; set; }
  public string? Type { get; set; }
}
