using Logitar.Identity.Contracts;

namespace Logitar.Portal.Contracts.Senders;

public record UpdateSenderPayload
{
  public string? EmailAddress { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public MailgunSettings? Mailgun { get; set; }
  public SendGridSettings? SendGrid { get; set; }
}
