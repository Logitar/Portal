using Logitar.Portal.Core.Emails.Messages;

namespace Logitar.Portal.Core.Emails.Providers
{
  public interface IProviderStrategy
  {
    IMessageHandler Execute(IReadOnlyDictionary<string, string?> settings);
  }
}
