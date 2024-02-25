using Logitar.Net.Mail;
using Logitar.Net.Mail.Mailgun;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders.Mailgun;

namespace Logitar.Portal.Infrastructure.Messages.Providers.Mailgun;

internal class MailgunHandler : IMessageHandler
{
  private readonly MailgunClient _client;

  public MailgunHandler(ReadOnlyMailgunSettings settings)
  {
    _client = new(settings.ApiKey, settings.DomainName);
  }

  public void Dispose()
  {
    _client.Dispose();
  }

  public async Task<SendMailResult> SendAsync(MessageAggregate aggregate, CancellationToken cancellationToken)
  {
    MailMessage message = aggregate.ToMailMessage();
    SendMailResult result = await _client.SendAsync(message, cancellationToken);
    return result;
  }
}
