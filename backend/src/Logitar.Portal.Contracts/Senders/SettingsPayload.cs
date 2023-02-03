namespace Logitar.Portal.Contracts.Senders
{
  public record SettingPayload
  {
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
  }
}
