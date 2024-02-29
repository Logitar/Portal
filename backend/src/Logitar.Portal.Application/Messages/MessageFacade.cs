using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Application.Messages.Queries;
using Logitar.Portal.Application.Pipeline;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Application.Messages;

internal class MessageFacade : IMessageService
{
  private readonly IRequestPipeline _pipeline;

  public MessageFacade(IRequestPipeline pipeline)
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
    return await _pipeline.ExecuteAsync(new SendMessageInternalCommand(payload), cancellationToken);
  }
}
