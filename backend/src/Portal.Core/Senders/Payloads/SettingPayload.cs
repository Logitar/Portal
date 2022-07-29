namespace Portal.Core.Senders.Payloads
{
  public class SettingPayload
  {
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
  }
}
