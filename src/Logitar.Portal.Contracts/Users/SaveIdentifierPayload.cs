namespace Logitar.Portal.Contracts.Users;

public record SaveIdentifierPayload
{
  public SaveIdentifierPayload() : this(string.Empty, string.Empty)
  {
  }
  public SaveIdentifierPayload(string key, string value)
  {
    Key = key;
    Value = value;
  }

  public string Key { get; set; }
  public string Value { get; set; }
}
