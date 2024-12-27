namespace Logitar.Portal.Contracts.Senders;

public record UpdateSenderPayload
{
  public string? EmailAddress { get; set; }
  public string? PhoneNumber { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public MailgunSettings? Mailgun { get; set; }
  public SendGridSettings? SendGrid { get; set; }
  public TwilioSettings? Twilio { get; set; }
}
