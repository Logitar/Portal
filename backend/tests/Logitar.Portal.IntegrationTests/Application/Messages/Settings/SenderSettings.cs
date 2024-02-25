using Logitar.Portal.Application.Messages.Settings;

namespace Logitar.Portal.Application.Messages;

internal record SenderSettings
{
  public MailgunTestSettings Mailgun { get; set; }
  public SendGridTestSettings SendGrid { get; set; }

  public string? DisplayName { get; set; }

  public SenderSettings() : this(new MailgunTestSettings(), new SendGridTestSettings())
  {
  }

  public SenderSettings(MailgunTestSettings mailgun, SendGridTestSettings sendGrid)
  {
    Mailgun = mailgun;
    SendGrid = sendGrid;
  }
}
