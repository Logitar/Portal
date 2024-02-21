using Logitar.Net.Mail;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages.Providers;

internal interface IMessageHandler : IDisposable
{
  Task<SendMailResult> SendAsync(MessageAggregate message, CancellationToken cancellationToken = default);
}
