using Logitar.Identity.Domain.Users;
using Logitar.Net.Sms;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Infrastructure.Messages;

internal static class MessageExtensions
{
  public static string ToE164PhoneNumber(this SenderSummary sender)
  {
    if (sender.Phone == null)
    {
      throw new ArgumentException($"The sender must be a {nameof(SenderType.Sms)} sender in order to be converted to an E.164 phone number.", nameof(sender));
    }
    return sender.Phone.FormatToE164();
  }

  public static SmsMessage ToSmsMessage(this Message aggregate)
  {
    Recipient[] recipients = aggregate.Recipients.Where(recipient => recipient.Type == RecipientType.To).ToArray();
    if (recipients.Length != 1)
    {
      throw new ArgumentException($"Exactly one {nameof(RecipientType.To)} recipient must be provided.", nameof(aggregate));
    }
    Recipient recipient = recipients.Single();
    if (recipient.PhoneNumber == null)
    {
      throw new ArgumentException($"The recipient requires a {nameof(Recipient.PhoneNumber)} to receive a SMS message.", nameof(aggregate));
    }

    if (aggregate.Body.Type != MediaTypeNames.Text.Plain)
    {
      throw new ArgumentException($"The SMS message text contents must be '{MediaTypeNames.Text.Plain}'. The content type '{aggregate.Body.Type}' is not supported.", nameof(aggregate));
    }

    return new SmsMessage(from: aggregate.Sender.ToE164PhoneNumber(), to: recipient.PhoneNumber, body: aggregate.Body.Text);
  }

  public static MailMessage ToMailMessage(this Message message)
  {
    MailMessage mailMessage = new()
    {
      From = message.Sender.ToMailAddress(),
      Subject = message.Subject.Value,
      Body = message.Body.Text,
      IsBodyHtml = message.Body.Type == MediaTypeNames.Text.Html
    };

    foreach (Recipient recipient in message.Recipients)
    {
      MailAddress address = recipient.ToMailAddress();
      switch (recipient.Type)
      {
        case RecipientType.Bcc:
          mailMessage.Bcc.Add(address);
          break;
        case RecipientType.CC:
          mailMessage.CC.Add(address);
          break;
        case RecipientType.To:
          mailMessage.To.Add(address);
          break;
      }
    }

    return mailMessage;
  }

  public static MailAddress ToMailAddress(this SenderSummary sender)
  {
    if (sender.Email == null)
    {
      throw new ArgumentException($"The sender must be an {nameof(SenderType.Email)} sender in order to be converted into a {nameof(MailAddress)}.", nameof(sender));
    }
    return new(sender.Email.Address, sender.DisplayName?.Value);
  }

  public static MailAddress ToMailAddress(this Recipient recipient)
  {
    if (recipient.Address == null)
    {
      throw new ArgumentException($"A recipient requires an email address in order to be converted into a {nameof(MailAddress)}.", nameof(recipient));
    }
    return new(recipient.Address, recipient.DisplayName);
  }
}
