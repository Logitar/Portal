namespace Logitar.Portal.Infrastructure.Messages.Providers;

public interface IProviderStrategy
{
  IMessageHandler Execute(IReadOnlyDictionary<string, string> settings);
}
