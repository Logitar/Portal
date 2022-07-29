using Microsoft.Extensions.DependencyInjection;
using Portal.Core.Emails.Providers;
using Portal.Core.Emails.Senders;

namespace Portal.Core.Emails.Messages
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
      ArgumentNullException.ThrowIfNull(sender);

      IProviderStrategy strategy = sender.Provider switch
      {
        ProviderType.SendGrid => _serviceProvider.GetRequiredService<ISendGridStrategy>(),
        _ => throw new NotSupportedException($"The sender provider '{sender.Provider}' is not supported."),
      };

      return strategy.Execute(sender.Settings);
    }
  }
}
