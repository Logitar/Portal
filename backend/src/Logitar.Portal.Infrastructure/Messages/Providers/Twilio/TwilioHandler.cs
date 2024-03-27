using Logitar.Net.Mail;
using Logitar.Net.Sms;
using Logitar.Net.Sms.Twilio;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders.Twilio;

namespace Logitar.Portal.Infrastructure.Messages.Providers.Twilio;

internal class TwilioHandler : IMessageHandler
{
  private readonly TwilioClient _client;

  public TwilioHandler(ReadOnlyTwilioSettings settings)
  {
    _client = new(settings.AccountSid, settings.AuthenticationToken);
  }

  public void Dispose()
  {
    _client.Dispose();
  }

  public async Task<SendMailResult> SendAsync(MessageAggregate aggregate, CancellationToken cancellationToken)
  {
    SmsMessage message = aggregate.ToSmsMessage();
    SendSmsResult result = await _client.SendAsync(message, cancellationToken);
    return new SendMailResult(result.Succeeded, result.Data.ToDictionary());
  }
}
