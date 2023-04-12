using Logitar.Portal.v2.Core.Messages;

namespace Logitar.Portal.v2.Infrastructure.Messages;

public interface IMessageHandler : IDisposable
{
  Task<SendMessageResult> SendAsync(MessageAggregate message, CancellationToken cancellationToken = default);
}
