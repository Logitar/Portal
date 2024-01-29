namespace Logitar.Portal.Contracts.Errors;

public record ErrorData
{
  public string Key { get; set; }
  public string? Value { get; set; }

  public ErrorData() : this(string.Empty, null)
  {
  }

  public ErrorData(KeyValuePair<string, string?> pair) : this(pair.Key, pair.Value)
  {
  }

  public ErrorData(string key, string? value)
  {
    Key = key;
    Value = value;
  }
}
