namespace Portal.Core.Emails.Messages.Payloads
{
  public class RecipientPayload
  {
    public RecipientType Type { get; set; }
    public string? User { get; set; }

    public string? Address { get; set; }
    public string? DisplayName { get; set; }
  }
}
