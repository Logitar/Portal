using Logitar.Portal.Application.Messages.Settings;

namespace Logitar.Portal.Application.Messages;

internal record SenderSettings
{
  public MailgunTestSettings Mailgun { get; set; }
  public SendGridTestSettings SendGrid { get; set; }
  public TwilioTestSettings Twilio { get; set; }

  public string? DisplayName { get; set; }

  public SenderSettings() : this(new MailgunTestSettings(), new SendGridTestSettings(), new TwilioTestSettings())
  {
  }

  public SenderSettings(MailgunTestSettings mailgun, SendGridTestSettings sendGrid, TwilioTestSettings twilio)
  {
    Mailgun = mailgun;
    SendGrid = sendGrid;
    Twilio = twilio;
  }
}
