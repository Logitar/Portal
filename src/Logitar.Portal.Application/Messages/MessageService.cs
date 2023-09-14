using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages;

internal class MessageService : IMessageService
{
  private readonly IRequestPipeline _pipeline;

  public MessageService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SendMessageCommand(payload), cancellationToken);
  }
}
