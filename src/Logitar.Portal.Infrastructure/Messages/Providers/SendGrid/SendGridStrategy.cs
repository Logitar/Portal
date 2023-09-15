namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridStrategy : ISendGridStrategy
{
  public IMessageHandler Execute(IReadOnlyDictionary<string, string> settings)
  {
    SendGridSettings providerSettings = new(settings);

    return new SendGridHandler(providerSettings);
  }
}
