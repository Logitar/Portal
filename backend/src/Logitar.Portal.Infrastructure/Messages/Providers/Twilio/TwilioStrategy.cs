using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Twilio;

namespace Logitar.Portal.Infrastructure.Messages.Providers.Twilio;

internal class TwilioStrategy : IProviderStrategy
{
  public SenderProvider Provider { get; } = SenderProvider.Twilio;

  public IMessageHandler Execute(SenderSettings settings)
  {
    return new TwilioHandler((ReadOnlyTwilioSettings)settings);
  }
}
