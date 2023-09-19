using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Infrastructure.Messages.Providers;

internal interface IProviderStrategy
{
  ProviderType Type { get; }

  IMessageHandler Execute(IReadOnlyDictionary<string, string> settings);
}
