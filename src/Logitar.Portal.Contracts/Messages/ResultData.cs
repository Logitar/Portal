namespace Logitar.Portal.Contracts.Messages;

public record ResultData
{
  public ResultData() : this(string.Empty, string.Empty)
  {
  }
  public ResultData(string key, string value)
  {
    Key = key;
    Value = value;
  }

  public string Key { get; set; }
  public string Value { get; set; }
}
