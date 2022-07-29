namespace Portal.Core.Senders.Payloads
{
  public class CreateSenderPayload : SaveSenderPayload
  {
    public string? Realm { get; set; }

    public ProviderType Provider { get; set; }
  }
}
