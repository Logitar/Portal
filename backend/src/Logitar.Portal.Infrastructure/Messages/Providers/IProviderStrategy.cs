using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Infrastructure.Messages.Providers;

internal interface IProviderStrategy
{
  SenderProvider Provider { get; }

  IMessageHandler Execute(SenderSettings settings);
}
