using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridStrategy : IProviderStrategy
{
  public ProviderType Type { get; } = ProviderType.SendGrid;

  public IMessageHandler Execute(IReadOnlyDictionary<string, string> settings)
  {
    SendGridSettings providerSettings = new(settings);

    return new SendGridHandler(providerSettings);
  }
}
