using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Application.Messages.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages;

internal class MessageService : IMessageService
{
  private readonly IRequestPipeline _pipeline;

  public MessageService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadMessageQuery(id), cancellationToken);
  }

  public async Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SearchMessagesQuery(payload), cancellationToken);
  }

  public async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SendMessageCommand(payload), cancellationToken);
  }

  public async Task<Message> SendDemoAsync(SendDemoMessagePayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SendDemoMessageCommand(payload), cancellationToken);
  }
}
