using Logitar.Portal.v2.Contracts.Senders;
using Logitar.Portal.v2.Core.Senders;
using Logitar.Portal.v2.Infrastructure.Messages.Providers;
using Logitar.Portal.v2.Infrastructure.Messages.Providers.SendGrid;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.v2.Infrastructure.Messages;

internal class MessageHandlerFactory : IMessageHandlerFactory
{
  private readonly IServiceProvider _serviceProvider;

  public MessageHandlerFactory(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public IMessageHandler GetHandler(SenderAggregate sender)
  {
    IProviderStrategy strategy = sender.Provider switch
    {
      ProviderType.SendGrid => _serviceProvider.GetRequiredService<ISendGridStrategy>(),
      _ => throw new NotSupportedException($"The sender provider '{sender.Provider}' is not supported."),
    };

    return strategy.Execute(sender.Settings);
  }
}
