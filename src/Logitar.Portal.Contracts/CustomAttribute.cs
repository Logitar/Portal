namespace Logitar.Portal.Contracts;

public record CustomAttribute
{
  public CustomAttribute() : this(string.Empty, string.Empty)
  {
  }
  public CustomAttribute(string key, string value)
  {
    Key = key;
    Value = value;
  }

  public string Key { get; set; }
  public string Value { get; set; }
}
