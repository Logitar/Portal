using Logitar.Net.Mail;
using Logitar.Net.Mail.SendGrid;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders.SendGrid;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridHandler : IMessageHandler
{
  private readonly SendGridClient _client;

  public SendGridHandler(ReadOnlySendGridSettings settings)
  {
    _client = new(settings.ApiKey);
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
