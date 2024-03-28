using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders;

public static class SenderProviderExtensions
{
  public static SenderType GetSenderType(this SenderProvider provider)
  {
    return provider switch
    {
      SenderProvider.Mailgun or SenderProvider.SendGrid => SenderType.Email,
      SenderProvider.Twilio => SenderType.Sms,
      _ => throw new SenderProviderNotSupportedException(provider),
    };
  }

  public static bool IsEmailSender(this SenderProvider provider) => provider.GetSenderType() == SenderType.Email;
  public static bool IsSmsSender(this SenderProvider provider) => provider.GetSenderType() == SenderType.Sms;
}
