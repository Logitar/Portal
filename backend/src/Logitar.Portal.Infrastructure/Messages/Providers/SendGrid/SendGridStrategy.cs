using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridStrategy : IProviderStrategy
{
  public SenderProvider Provider { get; } = SenderProvider.SendGrid;

  public IMessageHandler Execute(SenderSettings settings)
  {
    return new SendGridHandler((ReadOnlySendGridSettings)settings);
  }
}
