using Logitar.Portal.Core.Messages;

namespace Logitar.Portal.Infrastructure.Messages;

public interface IMessageHandler : IDisposable
{
  Task<SendMessageResult> SendAsync(MessageAggregate message, CancellationToken cancellationToken = default);
}
