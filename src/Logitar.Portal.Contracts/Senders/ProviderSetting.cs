namespace Logitar.Portal.Contracts.Senders;

public record ProviderSetting
{
  public ProviderSetting() : this(string.Empty, string.Empty)
  {
  }
  public ProviderSetting(string key, string value)
  {
    Key = key;
    Value = value;
  }

  public string Key { get; set; }
  public string Value { get; set; }
}
