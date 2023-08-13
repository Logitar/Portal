namespace Logitar.Portal.Core.Emails.Senders.Models
{
  public class SenderSummary : AggregateSummary
  {
    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }

    public bool IsDefault { get; set; }
    public ProviderType Provider { get; set; }
  }
}
