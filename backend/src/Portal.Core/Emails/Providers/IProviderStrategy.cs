using Portal.Core.Emails.Messages;

namespace Portal.Core.Emails.Providers
{
  public interface IProviderStrategy
  {
    IMessageHandler Execute(IReadOnlyDictionary<string, string?> settings);
  }
}
