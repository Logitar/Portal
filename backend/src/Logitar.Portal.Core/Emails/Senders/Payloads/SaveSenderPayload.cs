namespace Logitar.Portal.Core.Emails.Senders.Payloads
{
  public abstract class SaveSenderPayload
  {
    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }

    public IEnumerable<SettingPayload>? Settings { get; set; }
  }
}
