namespace Logitar.Portal.v2.Infrastructure.Messages.Providers;

public interface IProviderStrategy
{
  IMessageHandler Execute(IReadOnlyDictionary<string, string> settings);
}
