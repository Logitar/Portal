using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages;

internal interface IMessageHandler : IDisposable
{
  Task<SendMessageResult> SendAsync(MessageAggregate message, CancellationToken cancellationToken = default);
}
