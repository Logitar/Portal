using Logitar.Portal.Application.Emails.Messages;

namespace Logitar.Portal.Application.Emails.Providers
{
  public interface IProviderStrategy
  {
    IMessageHandler Execute(IReadOnlyDictionary<string, string?> settings);
  }
}
