namespace Logitar.Portal.Infrastructure.Messages.Providers;

internal interface IProviderStrategy
{
  IMessageHandler Execute(IReadOnlyDictionary<string, string> settings);
}
