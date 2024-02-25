using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;

namespace Logitar.Portal.Infrastructure.Messages.Providers.Mailgun;

internal class MailgunStrategy : IProviderStrategy
{
  public SenderProvider Provider { get; } = SenderProvider.Mailgun;

  public IMessageHandler Execute(SenderSettings settings)
  {
    return new MailgunHandler((ReadOnlyMailgunSettings)settings);
  }
}
