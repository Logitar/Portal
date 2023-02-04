using Logitar.Portal.Application.Messages.Providers;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Messages
{
  internal class MessageHandlerFactory : IMessageHandlerFactory
  {
    private readonly IServiceProvider _serviceProvider;

    public MessageHandlerFactory(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    public IMessageHandler GetHandler(Sender sender)
    {
      IProviderStrategy strategy = sender.Provider switch
      {
        ProviderType.SendGrid => _serviceProvider.GetRequiredService<ISendGridStrategy>(),
        _ => throw new NotSupportedException($"The sender provider '{sender.Provider}' is not supported."),
      };

      return strategy.Execute(sender.Settings);
    }
  }
}
