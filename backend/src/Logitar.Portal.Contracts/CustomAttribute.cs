namespace Logitar.Portal.Contracts;

public record CustomAttribute
{
  public string Key { get; set; }
  public string Value { get; set; }

  public CustomAttribute() : this(string.Empty, string.Empty)
  {
  }

  public CustomAttribute(KeyValuePair<string, string> pair) : this(pair.Key, pair.Value)
  {
  }

  public CustomAttribute(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
