using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Senders;

public record ReplaceSenderPayload
{
  public EmailPayload Email { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public SendGridSettings SendGrid { get; set; }

  public ReplaceSenderPayload() : this(email: new(), sendGrid: new())
  {
  }

  public ReplaceSenderPayload(EmailPayload email, SendGridSettings sendGrid)
  {
    Email = email;
    SendGrid = sendGrid;
  }
}
