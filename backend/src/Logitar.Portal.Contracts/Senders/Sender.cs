using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Senders;

public class Sender : Aggregate
{
  public bool IsDefault { get; set; }

  public string? EmailAddress { get; set; }
  public string? PhoneNumber { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public SenderProvider Provider { get; set; }
  public MailgunSettings? Mailgun { get; set; }
  public SendGridSettings? SendGrid { get; set; }
  public TwilioSettings? Twilio { get; set; }

  public Realm? Realm { get; set; }

  public Sender() : this(string.Empty)
  {
  }

  public Sender(string emailAddress)
  {
    EmailAddress = emailAddress;
  }
}
