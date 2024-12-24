using Logitar.Identity.Core.Users;
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

  public static SmsMessage ToSmsMessage(this MessageAggregate aggregate)
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
}
