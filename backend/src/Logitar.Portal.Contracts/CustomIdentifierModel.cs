namespace Logitar.Portal.Contracts;

public record CustomIdentifierModel
{
  public string Key { get; set; }
  public string Value { get; set; }

  public CustomIdentifierModel() : this(string.Empty, string.Empty)
  {
  }

  public CustomIdentifierModel(KeyValuePair<string, string> pair) : this(pair.Key, pair.Value)
  {
  }

  public CustomIdentifierModel(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
