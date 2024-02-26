using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Senders;

public record ReplaceSenderPayload
{
  public EmailPayload Email { get; set; } // TODO(fpion): EmailAddress
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public MailgunSettings? Mailgun { get; set; }
  public SendGridSettings? SendGrid { get; set; }

  public ReplaceSenderPayload() : this(new EmailPayload())
  {
  }

  public ReplaceSenderPayload(EmailPayload email)
  {
    Email = email;
  }
}
