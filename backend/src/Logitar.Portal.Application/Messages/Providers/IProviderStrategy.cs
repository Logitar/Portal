namespace Logitar.Portal.Application.Messages.Providers
{
  public interface IProviderStrategy
  {
    IMessageHandler Execute(IReadOnlyDictionary<string, string?> settings);
  }
}
