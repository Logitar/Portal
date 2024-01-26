namespace Logitar.Portal.Contracts;

public record CustomAttributeModification
{
  public string Key { get; set; }
  public string? Value { get; set; }

  public CustomAttributeModification() : this(string.Empty, value: null)
  {
  }

  public CustomAttributeModification(KeyValuePair<string, string?> customAttribute) : this(customAttribute.Key, customAttribute.Value)
  {
  }

  public CustomAttributeModification(string key, string? value)
  {
    Key = key;
    Value = value;
  }
}
