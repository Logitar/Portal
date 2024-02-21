namespace Logitar.Portal.Contracts.Messages;

public record ResultData
{
  public string Key { get; set; }
  public string Value { get; set; }

  public ResultData() : this(string.Empty, string.Empty)
  {
  }

  public ResultData(KeyValuePair<string, string> data) : this(data.Key, data.Value)
  {
  }

  public ResultData(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
