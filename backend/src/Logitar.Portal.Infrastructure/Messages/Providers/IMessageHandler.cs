using Logitar.Net.Mail;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages.Providers;

internal interface IMessageHandler : IDisposable
{
  Task<SendMailResult> SendAsync(Message message, CancellationToken cancellationToken = default);
}
