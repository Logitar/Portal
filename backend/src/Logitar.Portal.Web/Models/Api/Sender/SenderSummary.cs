using Logitar.Portal.Core.Emails.Senders;

namespace Logitar.Portal.Web.Models.Api.Sender
{
  public class SenderSummary
  {
    public Guid Id { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }

    public bool IsDefault { get; private set; }
    public ProviderType Provider { get; set; }
  }
}
