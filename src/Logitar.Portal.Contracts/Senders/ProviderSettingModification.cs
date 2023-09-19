namespace Logitar.Portal.Contracts.Senders;

public record ProviderSettingModification
{
  public ProviderSettingModification() : this(string.Empty, null)
  {
  }
  public ProviderSettingModification(string key, string? value)
  {
    Key = key;
    Value = value;
  }

  public string Key { get; set; }
  public string? Value { get; set; }
}
