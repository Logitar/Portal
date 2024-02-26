namespace Logitar.Portal.Contracts;

public record CustomAttributeModification
{
  public string Key { get; set; }
  public string? Value { get; set; }

  public CustomAttributeModification() : this(string.Empty, null)
  {
  }

  public CustomAttributeModification(KeyValuePair<string, string?> pair) : this(pair.Key, pair.Value)
  {
  }

  public CustomAttributeModification(string key, string? value)
  {
    Key = key;
    Value = value;
  }
}
